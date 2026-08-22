using Shopniu_shared.Common;
using Shopniu_api.Aplication.Dashboard.UseCases.GetDashboardSummary;
using Shopniu_api.Domain.Entities.Dashboard;

namespace Shopniu_api.Aplication.Dashboard;

public class DashboardHandler
{
    private readonly GetDashboardSummaryUseCase _getDashboardSummaryUseCase;

    public DashboardHandler(GetDashboardSummaryUseCase getDashboardSummaryUseCase)
    {
        _getDashboardSummaryUseCase = getDashboardSummaryUseCase;
    }

    public async Task<ApiResponse<DashboardSummary>> GetSummaryAsync()
    {
        var result = await _getDashboardSummaryUseCase.ExecuteAsync();
        return ApiResponse<DashboardSummary>.Ok(result, "Dashboard Summary Retrieved Successfully");
    }
}
