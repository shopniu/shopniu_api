using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Aplication.Payments.UseCases.ProcessPaymentWebhook;
using Shopniu_api.Aplication.Payments.Ports;

namespace Shopniu_api.Aplication.Payments;

public class PaymentsHandler
{
    private readonly ProcessPaymentWebhookUseCase _processWompiWebhookUseCase;
    private readonly IPaymentWebhookTranslator _paymentWebhookTranslator;

    public PaymentsHandler(ProcessPaymentWebhookUseCase processWompiWebhookUseCase, IPaymentWebhookTranslator paymentWebhookTranslator)
    {
        _processWompiWebhookUseCase = processWompiWebhookUseCase;
        _paymentWebhookTranslator = paymentWebhookTranslator;
    }

    public async Task ProcessWompiWebhookAsync(string rawPayload)
    {
        var request = _paymentWebhookTranslator.Translate(rawPayload);

        await _processWompiWebhookUseCase.ExecuteAsync(request);
    }
}