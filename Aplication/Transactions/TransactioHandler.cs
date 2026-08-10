
using Shopniu_shared.Common;
using Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;
using Shopniu_api.Aplication.Transactions.UseCases.VerifyTransactionStatus;

namespace Shopniu_api.Aplication.Transactions;

public class TransactionHandler
{
    private readonly CreateTransactionUseCase _createTransactionUseCase;
    private readonly VerifyTransactionStatusUseCase _verifyTransactionStatusUseCase;

    public TransactionHandler(CreateTransactionUseCase createTransactionUseCase, VerifyTransactionStatusUseCase verifyTransactionStatusUseCase)
    {
        _createTransactionUseCase = createTransactionUseCase;
        _verifyTransactionStatusUseCase = verifyTransactionStatusUseCase;
    }

    public async Task<ApiResponse<VerifyTransactionResponseDTO>> VerifyTransactionStatusAsync(string reference)
    {
        var transaction = await _verifyTransactionStatusUseCase.ExecuteAsync(reference);
        return ApiResponse<VerifyTransactionResponseDTO>.Ok(transaction, "Transaction Retrieved Successfully");
    }

    public async Task<ApiResponse<CreateTransactionResponse>> CreateTransactionAsync(CreateTransactionRequest dto)
    {
        var result = await _createTransactionUseCase.ExecuteAsync(dto);
        return ApiResponse<CreateTransactionResponse>.Ok(result, "Transaction Created Successfully");
    }
}