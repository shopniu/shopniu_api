using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Domain.Repositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<Transaction?> GetByReferenceAsync(string wompiReference);
    Task<Transaction> CreateAsync(Transaction transaction);

    Task<Transaction?> UpdateAsync(Transaction transaction);
    Task DeleteAsync(int transactionId, int deletedBy);
}
