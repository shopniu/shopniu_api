

namespace Shopniu_api.Aplication.Products.UseCases.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    decimal Price,
    string ImageUrl,
    string Description,
    int Stock
);
