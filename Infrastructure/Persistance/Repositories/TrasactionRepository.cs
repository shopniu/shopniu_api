using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Microsoft.EntityFrameworkCore;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetAllAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
    }

    public async Task<Transaction?> GetByReferenceAsync(string paymentReference)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionReference == paymentReference);
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);

        return transaction;
    }

    public async Task<Transaction?> UpdateAsync(Transaction transaction)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transaction.Id);
        if (existingTransaction == null)
        {
            return null;
        }

        _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);

        return existingTransaction;
    }

    public async Task DeleteAsync(int transactionId, int deletedBy)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction != null)
        {
            // Optionally, you can log the user who deleted the transaction using the deletedBy parameter.
            _context.Transactions.Remove(transaction);

        }
    }
}