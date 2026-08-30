
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Aplication.Products.UseCases.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    decimal Price,
    string ImageUrl,
    string Description,
    int Stock,
    ProductSourcing? Sourcing = null,
    bool? CertifiedOriginal = null,
    decimal? CostPrice = null,
    string? SupplierName = null,
    int? LeadTimeDays = null,
    int? SupplierId = null
);
