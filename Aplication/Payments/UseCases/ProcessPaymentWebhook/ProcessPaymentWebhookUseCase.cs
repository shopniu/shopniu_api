using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Transactions.UseCases.ProcessPaymentWebhook;
using Shopniu_api.Domain.Entities.TransactionEntity;


namespace Shopniu_api.Aplication.Payments.UseCases.ProcessPaymentWebhook;

public class ProcessPaymentWebhookUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentWebhookUseCase(
        ITransactionRepository transactionRepository,
        IDeliveryRepository deliveryRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _deliveryRepository = deliveryRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ProcessPaymentWebhookRequest payload)
    {
        var transaction = await _transactionRepository.GetByReferenceAsync(payload.ProviderReference);
        if (transaction == null)
        {
            throw new NotFoundException("Transaction", payload.ProviderReference);
        }

        // El descuento de stock solo aplica en la transición PENDING ->
        // COMPLETED: los webhooks del proveedor pueden repetirse y un evento
        // posterior no debe volver a descontar.
        var previousStatus = transaction.Status;

        transaction.UpdatePaymentResult(payload.ProviderTransactionId, payload.Status);

        // El delivery se actualiza según el resultado del pago (transacciones
        // previas sin delivery se omiten).
        var delivery = await _deliveryRepository.GetByTransactionIdAsync(transaction.Id);
        delivery?.UpdateStatusFromTransaction(payload.Status);

        // Pago aprobado por primera vez: se descuenta el stock según la
        // cantidad de cada orden de la transacción, dentro del mismo
        // SaveChanges atómico.
        if (payload.Status == TransactionStatus.COMPLETED && previousStatus == TransactionStatus.PENDING)
        {
            var orders = await _orderRepository.GetByTransactionIdAsync(transaction.Id);

            var products = (await _productRepository.GetByIdsAsync(
                    orders.Select(o => o.ProductId).Distinct().ToList()))
                .ToDictionary(p => p.Id);

            foreach (var order in orders)
            {
                if (!products.TryGetValue(order.ProductId, out var product))
                {
                    throw new NotFoundException("Product", order.ProductId);
                }
                product.DecreaseStock(order.Quantity);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
