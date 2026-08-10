using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Aplication.Transactions.Ports;

public record PaymentResponse(
    string Id,
    TransactionStatus Status,
    string Reference
);