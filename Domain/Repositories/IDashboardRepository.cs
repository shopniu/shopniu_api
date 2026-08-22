using Shopniu_api.Domain.Entities.Dashboard;

namespace Shopniu_api.Domain.Repositories;

public interface IDashboardRepository
{
    /// <summary>Indicadores agregados del panel para un usuario (solo lectura).</summary>
    Task<DashboardSummary> GetSummaryAsync(int userId);
}
