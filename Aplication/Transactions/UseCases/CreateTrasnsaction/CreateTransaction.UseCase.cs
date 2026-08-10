

using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Aplication.Transactions.Ports;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

public class CreateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;
    public CreateTransactionUseCase(
        ITransactionRepository transactionRepository,
        ICurrentUserService currentUserService,
        IProductRepository productRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
        _currentUser = currentUserService;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateTransactionResponse> ExecuteAsync(CreateTransactionRequest dto)
    {
        // Idempotency check: If a transaction with the same idempotency key exists, return it instead of creating a new one
        var existingTransaction = await _transactionRepository.GetByIdempotencyKeyAsync(dto.IdempotencyKey);
        if (existingTransaction != null)
        {
            return new CreateTransactionResponse(
                TransactionId: existingTransaction.Id,
                Status: existingTransaction.Status
            ); // Retornar la transacción existente si ya existe
        }
        // consultar los productos
        var products = (await _productRepository.GetByIdsAsync(dto.Products.Select(p => p.ProductId).ToList())).ToDictionary(p => p.Id);

        // validar existencia y stock, guardar products
        var productsWithQuantity = new List<ProductWithQuantity>();
        foreach (var productDto in dto.Products)
        {
            if (!products.TryGetValue(productDto.ProductId, out var product))
            {
                throw new NotFoundException(nameof(product), productDto.ProductId);
            }
            product.ValidateStock(productDto.Quantity);
            productsWithQuantity.Add(new ProductWithQuantity(productDto.Quantity, product));
        }


        var transaction = new Transaction(
            userId: dto.UserId,
            idempotencyKey: dto.IdempotencyKey,
            transactionReference: $"shopniu_{Guid.NewGuid():N}",
            status: TransactionStatus.PENDING
        );

        // Create orders by product
        var orders = dto.Products
        .Select(item => new Order(dto.UserId, item.ProductId, item.Quantity, transaction)).ToList();

        // get payment details
        var paymentDetails = PaymentDetails.Create(
            products: productsWithQuantity,
            deliveryInCents: 0,
            currency: dto.Currency,
            paymentMethod: dto.PaymentMethod,
            transaction: transaction
        );

        await _transactionRepository.CreateAsync(transaction);

        // Create payment request by payment gateway
        var paymentRequest = new PaymentRequest(
            AmountInCents: paymentDetails.TotalInCents,
            Currency: dto.Currency,
            Reference: transaction.TransactionReference,
            Email: _currentUser.Email,
            PaymentMethod: dto.PaymentMethod,
            ProviderToken: dto.ProviderToken,
            AcceptanceToken: dto.AcceptanceToken,
            AcceptancePersonalToken: dto.AcceptancePersonalToken
        );
        var paymentResponse = await _paymentGateway.CreatePayment(paymentRequest);

        // Update transaction with necesary data payment response
        transaction.UpdatePaymentResult(paymentResponse.Id, paymentResponse.Status);
        await _transactionRepository.UpdateAsync(transaction);

        await _unitOfWork.SaveChangesAsync();

        return new CreateTransactionResponse(
            TransactionId: transaction.Id,
            Status: transaction.Status
        );
    }
}