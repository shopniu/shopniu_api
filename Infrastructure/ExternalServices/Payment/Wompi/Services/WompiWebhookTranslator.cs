// Infrastructure/Adapters/Payment/Wompi/WompiWebhookTranslator.cs
using System.Text.Json;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Aplication.Payments.Ports;
using Shopniu_api.Aplication.Transactions.UseCases.ProcessPaymentWebhook;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.DTOs;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;

public class WompiWebhookTranslator : IPaymentWebhookTranslator
{
    private readonly WompiWebhookSignatureValidation _signatureValidation;

    public WompiWebhookTranslator(WompiWebhookSignatureValidation signatureValidation)
    {
        _signatureValidation = signatureValidation;
    }

    public ProcessPaymentWebhookRequest Translate(string rawPayload)
    {
        var payload = JsonSerializer.Deserialize<WompiWebhookPayload>(rawPayload)
            ?? throw new BusinessRuleException("Payload de webhook inválido.");

        if (!_signatureValidation.IsValid(payload))
        {
            throw new BusinessRuleException("Invalid signature for Wompi webhook payload.");
        }

        return new ProcessPaymentWebhookRequest(
            ProviderReference: payload.Data.Transaction.Reference,
            ProviderTransactionId: payload.Data.Transaction.Id,
            Status: WompiStatusMapper.Map(payload.Data.Transaction.Status)
        );
    }
}