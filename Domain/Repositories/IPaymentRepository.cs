using Shopniu_api.Domain.Entities.PaymentDetailsEntity;

namespace Shopniu_api.Domain.Repositories;

public interface IPaymentDetailsRepository
{
    Task<PaymentDetails> CreateAsync(PaymentDetails paymentDetails);
}