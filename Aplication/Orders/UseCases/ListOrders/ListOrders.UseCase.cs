using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Orders.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Aplication.Orders.UseCases.ListOrders;

public class ListOrdersUseCase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUser;

    public ListOrdersUseCase(IDeliveryRepository deliveryRepository, ICurrentUserService currentUser)
    {
        _deliveryRepository = deliveryRepository;
        _currentUser = currentUser;
    }

    /// <summary>Pedidos pagados con su delivery y items para el back-office.
    /// La política product.create ya garantiza sesión; userId 0 indica una
    /// misconfiguración de issuer/claims y se reporta como 401.</summary>
    public async Task<IEnumerable<OrderFulfillmentDTO>> ExecuteAsync(DeliveryStatus? status = null)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var deliveries = await _deliveryRepository.GetAllWithDetailsAsync();

        if (status.HasValue)
        {
            deliveries = deliveries.Where(d => d.Status == status.Value).ToList();
        }

        return OrderFulfillmentDTO.FromEntities(deliveries);
    }
}
