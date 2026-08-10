

using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

public sealed record TransactionProductRequest(
    int ProductId,
    int Quantity
);

public sealed record CreateTransactionRequest(
    int UserId,
    string IdempotencyKey,
    PaymentMethodType PaymentMethod,
    CurrencyType Currency,
    string ProviderToken,
    string AcceptanceToken,
    string AcceptancePersonalToken,
    List<TransactionProductRequest> Products
);

public sealed record CreateTransactionResponse(
    int TransactionId,
    TransactionStatus Status

);
