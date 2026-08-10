
using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Aplication.Transactions.UseCases.VerifyTransactionStatus;

public record VerifyTransactionResponseDTO(
    int Id,
    string TransactionReference,
    string IdempotencyKey,
    TransactionStatus Status
)
{
    public static VerifyTransactionResponseDTO FromEntity(Transaction transaction)
    {
        return new VerifyTransactionResponseDTO(
            transaction.Id,
            transaction.TransactionReference,
            transaction.IdempotencyKey,
            transaction.Status
        );
    }
}