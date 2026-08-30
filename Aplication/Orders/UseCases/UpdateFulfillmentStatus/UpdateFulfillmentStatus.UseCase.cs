using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Orders.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Aplication.Orders.UseCases.UpdateFulfillmentStatus;

public class UpdateFulfillmentStatusUseCase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateFulfillmentStatusUseCase(
        IDeliveryRepository deliveryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderFulfillmentDTO> ExecuteAsync(int transactionId, UpdateFulfillmentStatusRequest dto)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var delivery = await _deliveryRepository.GetByTransactionIdWithDetailsAsync(transactionId)
            ?? throw new NotFoundException("Delivery", transactionId);

        if (dto.Status == DeliveryStatus.SHIPPED)
        {
            delivery.MarkShipped(dto.TrackingNumber);
        }
        else if (dto.Status == DeliveryStatus.DELIVERED)
        {
            delivery.MarkDelivered();
        }
        else
        {
            throw new ValidationsException("Invalid fulfillment status.");
        }

        await _deliveryRepository.UpdateAsync(delivery);
        await _unitOfWork.SaveChangesAsync();
        return OrderFulfillmentDTO.FromEntity(delivery);
    }
}
