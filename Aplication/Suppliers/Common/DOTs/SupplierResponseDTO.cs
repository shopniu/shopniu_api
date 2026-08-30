using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Aplication.Suppliers.Common.DTOs;

public sealed record SupplierRequest(
    string Name,
    string? Region,
    decimal DefaultShipping,
    int DefaultLeadTimeDays,
    bool IsActive = true
);

public sealed record SupplierSyncSummaryDTO(
    DateTime RunAt,
    bool Succeeded,
    int Created,
    int Updated,
    int ErrorCount
);

public sealed record SupplierResponseDTO(
    int Id,
    string Name,
    string? Region,
    decimal DefaultShipping,
    int DefaultLeadTimeDays,
    bool IsActive,
    int ProductCount,
    SupplierSyncSummaryDTO? LastSync = null
)
{
    public static SupplierResponseDTO FromEntity(Supplier supplier, SupplierSyncSummaryDTO? lastSync = null)
    {
        return new SupplierResponseDTO(
            supplier.Id,
            supplier.Name,
            supplier.Region,
            supplier.DefaultShipping,
            supplier.DefaultLeadTimeDays,
            supplier.IsActive,
            supplier.Products?.Count ?? 0,
            lastSync
        );
    }

    public static IEnumerable<SupplierResponseDTO> FromEntities(IEnumerable<Supplier> suppliers)
    {
        return suppliers.Select(supplier => FromEntity(supplier));
    }

    public static async Task<IEnumerable<SupplierResponseDTO>> FromEntitiesAsync(
        IEnumerable<Supplier> suppliers,
        Func<Supplier, Task<SupplierSyncSummaryDTO?>> lastSyncResolver)
    {
        var result = new List<SupplierResponseDTO>();
        foreach (var supplier in suppliers)
        {
            var lastSync = await lastSyncResolver(supplier);
            result.Add(FromEntity(supplier, lastSync));
        }
        return result;
    }
}
