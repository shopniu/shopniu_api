
using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;


namespace Shopniu_api.Domain.Entities.TransactionEntity;

// enum for status
public enum TransactionStatus
{
    PENDING,
    COMPLETED,
    FAILED,
    CANCELED,
    REFUNDED
}


public class Transaction : BaseEntity
{
    public int UserId { get; set; }
    public string IdempotencyKey { get; set; } = null!;
    public TransactionStatus Status { get; set; } = TransactionStatus.PENDING;
    public string TransactionReference { get; set; } = null!;
    public string? ProviderTransactionId { get; set; }
    public PaymentDetails PaymentDetails { get; set; } = null!;
    public List<Order> Orders { get; set; } = new List<Order>();

    private Transaction() { }

    public Transaction(int userId, string idempotencyKey, TransactionStatus status, string transactionReference = "")
    {
        // userId 0 = compra sin cuenta registrada (guest checkout)
        if (userId < 0)
            throw new ValidationsException("User ID must be a non-negative integer.");
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ValidationsException("Idempotency key cannot be empty.");

        UserId = userId;
        IdempotencyKey = idempotencyKey;
        TransactionReference = transactionReference;
        Status = status;
    }

    public void UpdatePaymentResult(string providerTransactionId, TransactionStatus status)
    {
        ProviderTransactionId = providerTransactionId;
        Status = status;
    }
}