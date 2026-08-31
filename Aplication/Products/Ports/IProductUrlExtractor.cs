namespace Shopniu_api.Aplication.Products.Ports;

/// <summary>Datos extraídos de una URL de producto (para el flujo de
/// importación directa). El precio se interpreta como costo del proveedor.</summary>
public sealed record ExtractedProduct(
    string Name,
    string? ImageUrl,
    string? Description,
    decimal? Price,
    string? Brand = null
);

/// <summary>Resuelve información de un producto a partir de su URL en el
/// servidor (JSON-LD / OpenGraph). Implementaciones por fuente: genérica,
/// marketplace, etc.</summary>
public interface IProductUrlExtractor
{
    Task<ExtractedProduct?> ExtractAsync(string url, CancellationToken cancellationToken = default);
}
