namespace Shopniu_api.Aplication.Suppliers.Common.DTOs;

public sealed record SupplierSyncLogDTO(
    int Id,
    DateTime RunAt,
    bool Succeeded,
    int Created,
    int Updated,
    int ErrorCount,
    string? Errors
);
