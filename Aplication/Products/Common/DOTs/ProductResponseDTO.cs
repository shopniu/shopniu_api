

using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Aplication.Products.Common.DTOs;

public record ProductResponseDTO(
    int Id,
    string Name,
    decimal Price,
    string ImageUrl,
    string Description,
    int Stock,
    List<MediaAssetResponse>? Media = null,
    ProductSourcing Sourcing = ProductSourcing.LocalStock,
    bool CertifiedOriginal = false,
    string? SupplierName = null,
    int? LeadTimeDays = null,
    int? SupplierId = null,
    decimal? CostPrice = null
)
{
    /// <param name="includeMedia">Incluye la galería de imágenes.</param>
    /// <param name="includeInternal">Incluye el costo (solo flujos de
    /// back-office; el catálogo público jamás debe exponer el costo).</param>
    public static ProductResponseDTO FromEntity(Product product, bool includeMedia = false, bool includeInternal = false)
    {
        return new ProductResponseDTO(
            product.Id,
            product.Name,
            product.Price,
            product.ImageUrl,
            product.Description,
            product.Stock,
            includeMedia
                ? product.Media
                    .OrderByDescending(m => m.IsMain)
                    .ThenByDescending(m => m.CreatedAt)
                    .Select(MediaAssetResponse.FromEntity)
                    .ToList()
                : null,
            product.Sourcing,
            product.CertifiedOriginal,
            product.SupplierName,
            product.LeadTimeDays,
            product.SupplierId,
            includeInternal ? product.CostPrice : null
        );
    }

    public static IEnumerable<ProductResponseDTO> FromEntities(IEnumerable<Product> products, bool includeMedia = false, bool includeInternal = false)
    {
        return products.Select(product => FromEntity(product, includeMedia, includeInternal));
    }
}
