using Shopniu_api.Domain.Entities.common;

namespace Shopniu_api.Domain.Entities.SupplierEntity;

/// <summary>Registro de una corrida de sincronización del catálogo de un
/// proveedor. Permite monitorear éxito, errores y volúmenes en el tiempo.</summary>
public class SupplierSyncLog : BaseEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime RunAt { get; set; }
    public bool Succeeded { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public int ErrorCount { get; set; }

    /// <summary>Errores por ítem (un por línea).</summary>
    public string? Errors { get; set; }

    private SupplierSyncLog() { }

    public SupplierSyncLog(
        int supplierId,
        DateTime runAt,
        bool succeeded,
        int created,
        int updated,
        IReadOnlyList<string> errors)
    {
        SupplierId = supplierId;
        RunAt = runAt;
        Succeeded = succeeded;
        Created = created;
        Updated = updated;
        ErrorCount = errors.Count;
        Errors = errors.Count > 0 ? string.Join('\n', errors) : null;
    }
}
