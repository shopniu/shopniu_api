using Shopniu_api.Aplication.Products.Common.DTOs;

namespace Shopniu_api.Aplication.Products.UseCases.ImportProducts;

/// <summary>Ítem de catálogo externo a importar. El front manda el costo al
/// proveedor; el precio de venta lo calcula el back con el markup.</summary>
public sealed record ImportProductItem(
    string Name,
    decimal CostPrice,
    string ImageUrl,
    string Description,
    int Stock,
    string SupplierName,
    int LeadTimeDays
);

public sealed record ImportProductsRequest(List<ImportProductItem> Items);

public sealed record ImportProductsResult(
    int Created,
    List<string> Errors,
    List<ProductResponseDTO> Products
);
