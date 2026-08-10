namespace Shopniu_api.Aplication.Transactions.Ports;

public interface IPaymentGateway
{
    Task<PaymentResponse> CreatePayment(PaymentRequest paymentRequest);
}