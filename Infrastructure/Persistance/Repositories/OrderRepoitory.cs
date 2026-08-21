
using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);

        return order;
    }

    public async Task CreateRangeAsync(IEnumerable<Order> orders)
    {
        _context.Orders.AddRange(orders);

    }

    public async Task<List<Order>> GetByTransactionIdAsync(int transactionId)
    {
        return await _context.Orders
            .Where(o => o.TransactionId == transactionId)
            .ToListAsync();
    }
}