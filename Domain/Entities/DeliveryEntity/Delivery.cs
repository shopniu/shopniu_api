
using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Domain.Entities.DeliveryEntity;

public enum DeliveryStatus
{
    PENDING,
    SHIPPED,
    DELIVERED,
    CANCELLED
}

public class Delivery : BaseEntity
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public DeliveryStatus status { get; set; }
    public Transaction Transaction { get; set; } = null!;

    private Delivery() { }

    public Delivery(string address, string city, string state, string zipCode, string country, int userId, int transactionId)
    {
        Address = address;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
        UserId = userId;
        TransactionId = transactionId;
        status = DeliveryStatus.PENDING;
    }
}