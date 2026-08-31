using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Aplication.Products.UseCases.UpdateProduct;

/// <summary>Update completo (PUT). Los campos opcionales nullable permiten
/// dejar valores en null (ej. limpiar el proveedor); los que no aplican
/// simplemente se envían como su valor actual.</summary>
public sealed record UpdateProductRequest(
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
    int? SupplierId = null,
    decimal? MarkupPercent = null
);
