// Aplication/Transactions/UseCases/ProcessPaymentWebhook/ProcessPaymentWebhookRequest.cs
using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Aplication.Transactions.UseCases.ProcessPaymentWebhook;

public sealed record ProcessPaymentWebhookRequest(
    string ProviderReference,   // el "reference" que TÚ generaste (shopniu_xxx)
    string ProviderTransactionId,
    TransactionStatus Status
);