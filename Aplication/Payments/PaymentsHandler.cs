using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Aplication.Payments.UseCases.GetPaymentMethods;
using Shopniu_api.Aplication.Payments.UseCases.ProcessPaymentWebhook;
using Shopniu_api.Aplication.Payments.Ports;
using Shopniu_shared.Common;

namespace Shopniu_api.Aplication.Payments;

public class PaymentsHandler
{
    private readonly ProcessPaymentWebhookUseCase _processWompiWebhookUseCase;
    private readonly IPaymentWebhookTranslator _paymentWebhookTranslator;
    private readonly GetPaymentMethodsUseCase _getPaymentMethodsUseCase;

    public PaymentsHandler(
        ProcessPaymentWebhookUseCase processWompiWebhookUseCase,
        IPaymentWebhookTranslator paymentWebhookTranslator,
        GetPaymentMethodsUseCase getPaymentMethodsUseCase)
    {
        _processWompiWebhookUseCase = processWompiWebhookUseCase;
        _paymentWebhookTranslator = paymentWebhookTranslator;
        _getPaymentMethodsUseCase = getPaymentMethodsUseCase;
    }

    public async Task ProcessWompiWebhookAsync(string rawPayload)
    {
        var request = _paymentWebhookTranslator.Translate(rawPayload);

        await _processWompiWebhookUseCase.ExecuteAsync(request);
    }

    public async Task<ApiResponse<IEnumerable<UserPaymentMethodResponse>>> GetPaymentMethodsAsync()
    {
        var result = await _getPaymentMethodsUseCase.ExecuteAsync();
        return ApiResponse<IEnumerable<UserPaymentMethodResponse>>.Ok(result, "Payment Methods Retrieved Successfully");
    }
}