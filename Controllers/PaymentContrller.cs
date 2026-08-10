using Microsoft.AspNetCore.Mvc;
using Shopniu_api.Aplication.Payments;

[ApiController]
[Route("api/v1/[controller]")]
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
}