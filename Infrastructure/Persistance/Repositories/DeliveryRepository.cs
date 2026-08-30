using Shopniu_api.Domain.Entities.DeliveryEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly AppDbContext _context;

    public DeliveryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Delivery> CreateAsync(Delivery delivery)
    {
        _context.Deliveries.Add(delivery);

        return delivery;
    }

    public async Task<Delivery?> GetByTransactionIdAsync(int transactionId)
    {
        return await _context.Deliveries
            .FirstOrDefaultAsync(d => d.TransactionId == transactionId);
    }

    public async Task<List<Delivery>> GetAllWithDetailsAsync()
    {
        return await _context.Deliveries
            .Include(d => d.Transaction)
                .ThenInclude(t => t.Orders)
                .ThenInclude(o => o.Product)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Delivery?> GetByTransactionIdWithDetailsAsync(int transactionId)
    {
        return await _context.Deliveries
            .Include(d => d.Transaction)
                .ThenInclude(t => t.Orders)
                .ThenInclude(o => o.Product)
            .FirstOrDefaultAsync(d => d.TransactionId == transactionId);
    }

    public Task UpdateAsync(Delivery delivery)
    {
        _context.Deliveries.Update(delivery);
        return Task.CompletedTask;
    }
}