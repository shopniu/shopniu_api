namespace Shopniu_api.Domain.Entities.Dashboard;

/// <summary>
/// Read-model del resumen del panel: proyección agregada de solo lectura.
/// No es una entidad persistida (no tiene tabla ni migración propia).
/// </summary>
public record DashboardSummary(
    int TotalProducts,
    int TotalOrders,
    int PendingDispatchOrders,
    int DistinctBuyers);
