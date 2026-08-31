namespace Shopniu_api.Aplication.Products.UseCases.ExtractProductFromUrl;

public sealed record ExtractProductFromUrlRequest(string Url);

public sealed record ExtractedProductDTO(
    string Name,
    string? ImageUrl,
    string? Description,
    decimal? Price,
    string? Brand
);
