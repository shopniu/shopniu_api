using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shopniu_api.Aplication.Transactions;
using Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionHandler _transactionHandler;
    private readonly IValidator<CreateTransactionRequest> _validator;

    public TransactionController(
        TransactionHandler transactionHandler,
        IValidator<CreateTransactionRequest> validator)
    {
        _transactionHandler = transactionHandler;
        _validator = validator;
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
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new
            {
                detail = validation.Errors.FirstOrDefault()?.ErrorMessage,
                errors = validation.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray())
            });
        }

        var response = await _transactionHandler.CreateTransactionAsync(dto);
        return Ok(response);
    }
}