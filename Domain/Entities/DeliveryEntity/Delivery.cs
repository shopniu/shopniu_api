using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Domain.Exceptions.Common;

namespace Shopniu_api.Domain.Entities.DeliveryEntity;

public enum DeliveryStatus
{
    PENDING,
    ACTIVE,
    SHIPPED,
    DELIVERED,
    CANCELLED
}

public class Delivery : BaseEntity
{
    public const string DefaultCountry = "Colombia";

    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string DepartmentCode { get; set; } = null!;
    public string CityCode { get; set; } = null!;
    public DeliveryStatus Status { get; set; }
    public Transaction Transaction { get; set; } = null!;

    private Delivery() { }

    public Delivery(string address, string city, string state, string departmentCode, string cityCode, int userId, Transaction transaction)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ValidationsException("Address cannot be empty.");
        if (string.IsNullOrWhiteSpace(city))
            throw new ValidationsException("City cannot be empty.");
        if (string.IsNullOrWhiteSpace(state))
            throw new ValidationsException("State cannot be empty.");

        Address = address;
        City = city;
        State = state;
        DepartmentCode = departmentCode;
        CityCode = cityCode;
        UserId = userId;
        Transaction = transaction;
        TransactionId = transaction.Id;
        Status = DeliveryStatus.PENDING;
    }

    /// <summary>Actualiza el estado del envío según el resultado del pago.</summary>
    public void UpdateStatusFromTransaction(TransactionStatus transactionStatus)
    {
        Status = transactionStatus switch
        {
            TransactionStatus.COMPLETED => DeliveryStatus.ACTIVE,
            TransactionStatus.FAILED or TransactionStatus.CANCELED or TransactionStatus.REFUNDED => DeliveryStatus.CANCELLED,
            _ => DeliveryStatus.PENDING
        };
    }
}