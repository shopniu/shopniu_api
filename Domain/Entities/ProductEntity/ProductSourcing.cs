namespace Shopniu_api.Domain.Entities.ProductEntity;

/// <summary>Origen del inventario de un producto: stock local propio (despacho
/// inmediato) o despacho por un proveedor externo (dropshipping).</summary>
public enum ProductSourcing
{
    /// <summary>Stock propio en bodega: despacho inmediato.</summary>
    LocalStock,

    /// <summary>Se despacha por un proveedor externo con un tiempo estimado.</summary>
    External,
}
