using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Transactions.UseCases.ProcessPaymentWebhook;


namespace Shopniu_api.Aplication.Payments.UseCases.ProcessPaymentWebhook;

public class ProcessPaymentWebhookUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentWebhookUseCase(ITransactionRepository transactionRepository, IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ProcessPaymentWebhookRequest payload)
    {
        var transaction = await _transactionRepository.GetByReferenceAsync(payload.ProviderReference);
        if (transaction == null)
        {
            throw new NotFoundException("Transaction", payload.ProviderReference);
        }

        transaction.UpdatePaymentResult(payload.ProviderTransactionId, payload.Status);

        // El delivery se actualiza según el resultado del pago (transacciones
        // previas sin delivery se omiten).
        var delivery = await _deliveryRepository.GetByTransactionIdAsync(transaction.Id);
        delivery?.UpdateStatusFromTransaction(payload.Status);

        await _unitOfWork.SaveChangesAsync();
    }
}