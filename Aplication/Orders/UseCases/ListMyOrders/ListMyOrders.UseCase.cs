using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Orders.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Orders.UseCases.ListMyOrders;

/// <summary>Pedidos del usuario autenticado (comprador). Solo ve los suyos:
/// se resuelve desde el userId del token, nunca de la request.</summary>
public class ListMyOrdersUseCase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUser;

    public ListMyOrdersUseCase(IDeliveryRepository deliveryRepository, ICurrentUserService currentUser)
    {
        _deliveryRepository = deliveryRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<OrderFulfillmentDTO>> ExecuteAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var deliveries = await _deliveryRepository.GetByUserIdWithDetailsAsync(userId);
        return OrderFulfillmentDTO.FromEntities(deliveries);
    }
}
