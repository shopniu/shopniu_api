using Shopniu_api.Domain.Entities.OrderEntity;

namespace Shopniu_api.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task CreateRangeAsync(IEnumerable<Order> orders);
}