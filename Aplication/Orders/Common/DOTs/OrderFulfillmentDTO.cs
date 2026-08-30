using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Aplication.Orders.Common.DTOs;

public sealed record OrderFulfillmentItemDTO(
    int ProductId,
    string Name,
    string ImageUrl,
    decimal Price,
    int Quantity,
    string Sourcing,
    string? SupplierName,
    int? LeadTimeDays
);

public sealed record OrderFulfillmentDTO(
    int TransactionId,
    string TransactionReference,
    int UserId,
    string Status,
    string? TrackingNumber,
    string Address,
    string City,
    string Department,
    DateTime CreatedAt,
    decimal Total,
    List<OrderFulfillmentItemDTO> Items
)
{
    public static OrderFulfillmentDTO FromEntity(Delivery delivery)
    {
        var items = delivery.Transaction.Orders
            .Select(o => new OrderFulfillmentItemDTO(
                o.ProductId,
                o.Product.Name,
                o.Product.ImageUrl,
                o.Product.Price,
                o.Quantity,
                o.Product.Sourcing.ToString(),
                o.Product.SupplierName,
                o.Product.LeadTimeDays
            ))
            .ToList();

        return new OrderFulfillmentDTO(
            delivery.TransactionId,
            delivery.Transaction.TransactionReference,
            delivery.UserId,
            delivery.Status.ToString(),
            delivery.TrackingNumber,
            delivery.Address,
            delivery.City,
            delivery.Department,
            delivery.CreatedAt,
            items.Sum(item => item.Price * item.Quantity),
            items
        );
    }

    public static IEnumerable<OrderFulfillmentDTO> FromEntities(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Select(FromEntity);
    }
}

public sealed record UpdateFulfillmentStatusRequest(
    DeliveryStatus Status,
    string? TrackingNumber = null
);
