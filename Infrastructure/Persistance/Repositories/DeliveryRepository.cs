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
}