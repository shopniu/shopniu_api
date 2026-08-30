using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Aplication.Suppliers.Ports;

/// <summary>Fuente de catálogo de un proveedor (dropshipping). Cada proveedor
/// se implementa con su propio provider (CSV, API del proveedor, etc.).</summary>
public interface ISupplierCatalogProvider
{
    Task<IReadOnlyList<SupplierCatalogItem>> FetchAsync(
        Supplier supplier,
        CancellationToken cancellationToken = default);
}
