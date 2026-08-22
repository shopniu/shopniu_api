using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Domain.Entities.Dashboard;

namespace Shopniu_api.Aplication.Dashboard.UseCases.GetDashboardSummary;

public class GetDashboardSummaryUseCase
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardSummaryUseCase(IDashboardRepository dashboardRepository, ICurrentUserService currentUser)
    {
        _dashboardRepository = dashboardRepository;
        _currentUser = currentUser;
    }

    /// <summary>Resumen del panel para el usuario autenticado. La política
    /// product.create ya garantiza sesión; userId 0 indica una
    /// misconfiguración de issuer/claims y se reporta como 401.</summary>
    public async Task<DashboardSummary> ExecuteAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        return await _dashboardRepository.GetSummaryAsync(userId);
    }
}
