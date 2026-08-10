using Shopniu_api.Domain.Entities.PaymentDetailsEntity;

namespace Shopniu_api.Aplication.Transactions.Ports;

public record PaymentRequest(
    decimal AmountInCents,
    CurrencyType Currency,
    string? Email,
    string Reference,
    PaymentMethodType PaymentMethod,
    string ProviderToken,
    string AcceptanceToken,
    string AcceptancePersonalToken
);