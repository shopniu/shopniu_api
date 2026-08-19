using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Domain.Repositories;

public interface IDeliveryRepository
{
    Task<Delivery> CreateAsync(Delivery delivery);
    Task<Delivery?> GetByTransactionIdAsync(int transactionId);
}