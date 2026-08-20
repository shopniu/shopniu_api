using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopniu_api.Aplication.Payments;

[ApiController]
[Route("api/v1/payments")]
public class PaymentController : ControllerBase
{
    private readonly PaymentsHandler _paymentsHandler;

    public PaymentController(PaymentsHandler paymentsHandler)
    {
        _paymentsHandler = paymentsHandler;
    }

    [HttpPost("wompi-webhook")]
    public async Task<IActionResult> ProcessWompiWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var rawPayload = await reader.ReadToEndAsync();

        await _paymentsHandler.ProcessWompiWebhookAsync(rawPayload);
        return Ok();
    }

    /// <summary>Métodos de pago guardados del usuario autenticado (token).</summary>
    [Authorize]
    [HttpGet("methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        return Ok(await _paymentsHandler.GetPaymentMethodsAsync());
    }
}