

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
    List<MediaAssetResponse>? Media = null
)
{
    public static ProductResponseDTO FromEntity(Product product, bool includeMedia = false)
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
                : null
        );
    }

    public static IEnumerable<ProductResponseDTO> FromEntities(IEnumerable<Product> products, bool includeMedia = false)
    {
        return products.Select(product => FromEntity(product, includeMedia));
    }
}