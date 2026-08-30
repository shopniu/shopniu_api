using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Aplication.Suppliers.Common.DTOs;

public sealed record SupplierRequest(
    string Name,
    string? Region,
    decimal DefaultShipping,
    int DefaultLeadTimeDays,
    bool IsActive = true
);

public sealed record SupplierResponseDTO(
    int Id,
    string Name,
    string? Region,
    decimal DefaultShipping,
    int DefaultLeadTimeDays,
    bool IsActive,
    int ProductCount
)
{
    public static SupplierResponseDTO FromEntity(Supplier supplier)
    {
        return new SupplierResponseDTO(
            supplier.Id,
            supplier.Name,
            supplier.Region,
            supplier.DefaultShipping,
            supplier.DefaultLeadTimeDays,
            supplier.IsActive,
            supplier.Products?.Count ?? 0
        );
    }

    public static IEnumerable<SupplierResponseDTO> FromEntities(IEnumerable<Supplier> suppliers)
    {
        return suppliers.Select(FromEntity);
    }
}
