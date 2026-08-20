

using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

public sealed record TransactionProductRequest(
    int ProductId,
    int Quantity
);

public sealed record DeliveryRequest(
    string Address,
    string City,
    string Department,
    string DepartmentCode,
    string CityCode,
    string? Phone
);

public sealed record CreateTransactionRequest(
    string? CustomerEmail,
    string IdempotencyKey,
    PaymentMethodType PaymentMethod,
    CurrencyType Currency,
    string ProviderToken,
    string AcceptanceToken,
    string AcceptancePersonalToken,
    string? CardHolderName,
    int CardLastFour,
    DeliveryRequest Delivery,
    List<TransactionProductRequest> Products,
    bool SavePayment
);

public sealed record CreateTransactionResponse(
    int TransactionId,
    string Reference,
    TransactionStatus Status

);
