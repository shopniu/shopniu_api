using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.Dashboard;
using Shopniu_api.Domain.Entities.DeliveryEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummary> GetSummaryAsync(int userId)
    {
        var ownedProductIds = _context.ProductOwners
            .Where(po => po.UserId == userId)
            .Select(po => po.ProductId);

        var totalProducts = await _context.ProductOwners
            .CountAsync(po => po.UserId == userId);

        // "Pedidos" = compras (transacciones) que incluyen productos del usuario.
        var totalOrders = await _context.Orders
            .Where(o => ownedProductIds.Contains(o.ProductId))
            .Select(o => o.TransactionId)
            .Distinct()
            .CountAsync();

        // Pedidos pagados (delivery ACTIVE vía webhook) aún sin despachar.
        var pendingDispatchOrders = await (
            from o in _context.Orders
            join d in _context.Deliveries on o.TransactionId equals d.TransactionId
            where ownedProductIds.Contains(o.ProductId) && d.Status == DeliveryStatus.ACTIVE
            select o.TransactionId)
            .Distinct()
            .CountAsync();

        // Compradores registrados (excluye guest checkout con UserId == 0).
        var distinctBuyers = await _context.Orders
            .Where(o => ownedProductIds.Contains(o.ProductId) && o.UserId > 0)
            .Select(o => o.UserId)
            .Distinct()
            .CountAsync();

        return new DashboardSummary(
            totalProducts,
            totalOrders,
            pendingDispatchOrders,
            distinctBuyers);
    }
}
