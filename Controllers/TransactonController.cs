
using Microsoft.AspNetCore.Mvc;
using Shopniu_api.Aplication.Transactions;
using Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionHandler _transactionHandler;

    public TransactionController(TransactionHandler transactionHandler)
    {
        _transactionHandler = transactionHandler;
    }

    [HttpGet("{reference}/status")]
    public async Task<IActionResult> VerifyTransactionStatus(string reference)
    {
        var transaction = await _transactionHandler.VerifyTransactionStatusAsync(reference);
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest dto)
    {
        var response = await _transactionHandler.CreateTransactionAsync(dto);
        return Ok(response);
    }
}