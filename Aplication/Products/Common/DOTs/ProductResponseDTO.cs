

using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Aplication.Products.Common.DTOs;

public record ProductResponseDTO(
    int Id,
    string Name,
    decimal Price,
    string ImageUrl,
    string Description,
    int Stock
)
{
    public static ProductResponseDTO FromEntity(Product product)
    {
        return new ProductResponseDTO(
            product.Id,
            product.Name,
            product.Price,
            product.ImageUrl,
            product.Description,
            product.Stock
        );
    }

    public static IEnumerable<ProductResponseDTO> FromEntities(IEnumerable<Product> products)
    {
        return products.Select(FromEntity);
    }
}