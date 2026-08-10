
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;


namespace Shopniu_api.Aplication.Transactions.UseCases.VerifyTransactionStatus;

public class VerifyTransactionStatusUseCase
{
    private readonly ITransactionRepository _transactionRepository;

    public VerifyTransactionStatusUseCase(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<VerifyTransactionResponseDTO> ExecuteAsync(string wompiReference)
    {
        var transaction = await _transactionRepository.GetByReferenceAsync(wompiReference);
        if (transaction == null)
        {
            throw new NotFoundException("Transaction", wompiReference);
        }

        return VerifyTransactionResponseDTO.FromEntity(transaction);
    }
}