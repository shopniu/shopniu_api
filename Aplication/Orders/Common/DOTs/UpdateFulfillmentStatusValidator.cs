using FluentValidation;
using Shopniu_api.Aplication.Orders.Common.DTOs;
using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Aplication.Orders.Common.DTOs;

public class UpdateFulfillmentStatusValidator : AbstractValidator<UpdateFulfillmentStatusRequest>
{
    public UpdateFulfillmentStatusValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => status is DeliveryStatus.SHIPPED or DeliveryStatus.DELIVERED)
            .WithMessage("Only Shipped or Delivered statuses are supported.");

        RuleFor(x => x.TrackingNumber)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => x.Status == DeliveryStatus.SHIPPED)
            .WithMessage("Tracking number is required when marking as shipped.");
    }
}
