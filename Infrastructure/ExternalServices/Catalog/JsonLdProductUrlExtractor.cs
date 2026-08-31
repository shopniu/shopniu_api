using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using Shopniu_api.Aplication.Products.Ports;

namespace Shopniu_api.Infrastructure.ExternalServices.Catalog;

/// <summary>Extractor genérico por JSON-LD (schema Product) con fallback a
/// OpenGraph. Se ejecuta server-side; incluye guardia SSRF (solo http/https y
/// rechaza IPs privadas/reservadas) y timeout.</summary>
public partial class JsonLdProductUrlExtractor : IProductUrlExtractor
{
    private readonly HttpClient _httpClient;

    public JsonLdProductUrlExtractor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExtractedProduct?> ExtractAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("La URL debe ser http/https válida.", nameof(url));
        }

        if (await IsBlockedAddressAsync(uri.Host, cancellationToken))
        {
            throw new ArgumentException("La URL apunta a una dirección privada/reservada.", nameof(url));
        }

        using var response = await _httpClient.GetAsync(uri, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        return ExtractFromJsonLd(html, uri) ?? ExtractFromOpenGraph(html, uri);
    }

    private static async Task<bool> IsBlockedAddressAsync(string host, CancellationToken cancellationToken)
    {
        if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        IPAddress[] addresses;
        try
        {
            addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
        }
        catch (SocketException)
        {
            return true;
        }

        return addresses.All(IsPrivateOrReserved);
    }

    private static bool IsPrivateOrReserved(IPAddress ip)
    {
        if (ip.AddressFamily == AddressFamily.InterNetworkV6 && ip.IsIPv4MappedToIPv6)
        {
            ip = ip.MapToIPv4();
        }

        if (IPAddress.IsLoopback(ip))
        {
            return true;
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal;
        }

        var bytes = ip.GetAddressBytes();
        return bytes[0] == 10
            || (bytes[0] == 172 && bytes[1] is >= 16 and <= 31)
            || (bytes[0] == 192 && bytes[1] == 168)
            || (bytes[0] == 169 && bytes[1] == 254);
    }

    private static ExtractedProduct? ExtractFromJsonLd(string html, Uri pageUri)
    {
        foreach (Match match in JsonLdScriptRegex().Matches(html))
        {
            try
            {
                using var document = JsonDocument.Parse(match.Groups[1].Value);
                if (TryExtractProduct(document.RootElement, pageUri, out var product))
                {
                    return product;
                }
            }
            catch (JsonException)
            {
                // script malformado: se salta y se sigue con el resto
            }
        }

        return null;
    }

    private static bool TryExtractProduct(JsonElement element, Uri pageUri, out ExtractedProduct? product)
    {
        product = null;

        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    if (TryExtractProduct(item, pageUri, out product))
                    {
                        return product is not null;
                    }
                }
                return false;

            case JsonValueKind.Object:
                if (IsProductType(element))
                {
                    product = MapProduct(element, pageUri);
                    return product is not null;
                }

                if (element.TryGetProperty("@graph", out var graph))
                {
                    return TryExtractProduct(graph, pageUri, out product);
                }
                return false;

            default:
                return false;
        }
    }

    private static bool IsProductType(JsonElement element)
    {
        if (!element.TryGetProperty("@type", out var type))
        {
            return false;
        }

        return type.ValueKind switch
        {
            JsonValueKind.String => type.GetString() is "Product" or "IndividualProduct",
            JsonValueKind.Array => type.EnumerateArray()
                .Any(t => t.ValueKind == JsonValueKind.String
                    && t.GetString() is "Product" or "IndividualProduct"),
            _ => false,
        };
    }

    private static ExtractedProduct? MapProduct(JsonElement element, Uri pageUri)
    {
        var name = GetString(element, "name");
        var description = GetString(element, "description");
        var price = GetPrice(element);
        var brand = GetBrand(element);

        var imageUrl = GetImageUrl(element, pageUri);

        if (string.IsNullOrWhiteSpace(name) && imageUrl is null && price is null)
        {
            return null;
        }

        return new ExtractedProduct(
            name ?? "",
            imageUrl,
            description,
            price,
            brand);
    }

    private static string? GetString(JsonElement element, string property)
    {
        if (!element.TryGetProperty(property, out var value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            var text = value.GetString();
            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }

        if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty("@value", out var inner)
            && inner.ValueKind == JsonValueKind.String)
        {
            var text = inner.GetString();
            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }

        return null;
    }

    private static decimal? GetPrice(JsonElement element)
    {
        if (!element.TryGetProperty("offers", out var offers))
        {
            return null;
        }

        var offer = offers.ValueKind == JsonValueKind.Array
            ? offers.EnumerateArray().FirstOrDefault()
            : offers;

        if (offer.ValueKind != JsonValueKind.Object
            || !offer.TryGetProperty("price", out var price)
            || price.ValueKind != JsonValueKind.String
            || !decimal.TryParse(price.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return null;
        }

        return parsed;
    }

    private static string? GetBrand(JsonElement element)
    {
        if (!element.TryGetProperty("brand", out var brand) || brand.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return GetString(brand, "name");
    }

    private static string? GetImageUrl(JsonElement element, Uri pageUri)
    {
        if (!element.TryGetProperty("image", out var image))
        {
            return null;
        }

        string? url = image.ValueKind switch
        {
            JsonValueKind.String => image.GetString(),
            JsonValueKind.Array => image.EnumerateArray()
                .Select(item => ResolveImageUrl(item, pageUri))
                .FirstOrDefault(u => u is not null),
            _ => ResolveImageUrl(image, pageUri),
        };

        return string.IsNullOrWhiteSpace(url) ? null : url;
    }

    private static string? ResolveImageUrl(JsonElement image, Uri pageUri)
    {
        string? raw = image.ValueKind switch
        {
            JsonValueKind.String => image.GetString(),
            JsonValueKind.Object when image.TryGetProperty("url", out var url) && url.ValueKind == JsonValueKind.String => url.GetString(),
            _ => null,
        };

        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                ? raw
                : new Uri(pageUri, raw).ToString();
    }

    private static ExtractedProduct? ExtractFromOpenGraph(string html, Uri pageUri)
    {
        var title = MetaContent(html, "property=\"og:title\"");
        var image = MetaContent(html, "property=\"og:image\"");
        var description = MetaContent(html, "property=\"og:description\"");

        if (title is null && image is null)
        {
            return null;
        }

        if (image is not null && !image.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            image = new Uri(pageUri, image).ToString();
        }

        return new ExtractedProduct(title ?? "", image, description, null);
    }

    private static string? MetaContent(string html, string attribute)
    {
        var pattern = $@"<meta[^>]*{Regex.Escape(attribute)}[^>]*content=""([^""]*)""";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!match.Success)
        {
            return null;
        }

        var value = WebUtility.HtmlDecode(match.Groups[1].Value);
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    [GeneratedRegex(@"<script[^>]*type=""application/ld\+json""[^>]*>(.*?)</script>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex JsonLdScriptRegex();
}
