using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class PaymentDetailsRepository : IPaymentDetailsRepository
{
    private readonly AppDbContext _context;

    public PaymentDetailsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDetails> CreateAsync(PaymentDetails paymentDetails)
    {
        _context.PaymentDetails.Add(paymentDetails);

        return paymentDetails;
    }
}