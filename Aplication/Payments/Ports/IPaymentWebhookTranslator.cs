using Shopniu_api.Aplication.Transactions.UseCases.ProcessPaymentWebhook;


namespace Shopniu_api.Aplication.Payments.Ports;

public interface IPaymentWebhookTranslator
{
    ProcessPaymentWebhookRequest Translate(string rawPayload);
}