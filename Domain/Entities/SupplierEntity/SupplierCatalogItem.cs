namespace Shopniu_api.Domain.Entities.SupplierEntity;

/// <summary>Item del catálogo externo de un proveedor, tal como lo devuelve
/// un provider (CSV hoy, API de proveedor mañana).</summary>
public sealed record SupplierCatalogItem(
    string Sku,
    string Name,
    decimal CostPrice,
    int Stock,
    string ImageUrl,
    string Description,
    int LeadTimeDays
);
