namespace Shopniu_api.Aplication.Products.UseCases.UpdateProduct;

public sealed record UpdateProductRequest(
    string Name,
    decimal Price,
    string ImageUrl,
    string Description,
    int Stock
);
