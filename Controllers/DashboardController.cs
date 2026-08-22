using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Aplication.Dashboard;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardHandler _dashboardHandler;

    public DashboardController(DashboardHandler dashboardHandler)
    {
        _dashboardHandler = dashboardHandler;
    }

    // Resumen del panel para el usuario autenticado. Flujo de back-office:
    // admin y seller tienen product.create (mismo gate que /products/own).
    [Authorize(Policy = "product.create")]
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        return Ok(await _dashboardHandler.GetSummaryAsync());
    }
}
