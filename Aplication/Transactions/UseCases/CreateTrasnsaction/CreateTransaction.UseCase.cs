using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Aplication.Transactions.Ports;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Entities.DeliveryEntity;
using Shopniu_api.Domain.Entities.UserPaymentDataEntity;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Aplication.Common.Ports.CardEncryption;

namespace Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

public class CreateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUserPaymentDataRepository _userPaymentDataRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICardEncryptionService _cardEncryption;
    private readonly ILogger<CreateTransactionUseCase> _logger;
    public CreateTransactionUseCase(
        ITransactionRepository transactionRepository,
        ICurrentUserService currentUserService,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway,
        IUserPaymentDataRepository userPaymentDataRepository,
        IDeliveryRepository deliveryRepository,
        IUnitOfWork unitOfWork,
        ICardEncryptionService cardEncryption,
        ILogger<CreateTransactionUseCase> logger)
    {
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _currentUser = currentUserService;
        _paymentGateway = paymentGateway;
        _userPaymentDataRepository = userPaymentDataRepository;
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
        _cardEncryption = cardEncryption;
        _logger = logger;
    }

    public async Task<CreateTransactionResponse> ExecuteAsync(CreateTransactionRequest dto)
    {
        // Idempotency check: If a transaction with the same idempotency key exists, return it instead of creating a new one
        var existingTransaction = await _transactionRepository.GetByIdempotencyKeyAsync(dto.IdempotencyKey);
        if (existingTransaction != null)
        {
            return new CreateTransactionResponse(
                TransactionId: existingTransaction.Id,
                Reference: existingTransaction.TransactionReference,
                Status: existingTransaction.Status
            ); // Retornar la transacción existente si ya existe
        }

        // El usuario se resuelve desde el contexto (token); 0 = compra sin cuenta.
        var userId = _currentUser.UserId;
        // El email del cliente llega validado desde el front (zod/BFF) y se
        // resuelve en el controller; acá solo se usa para el pago con Wompi.
        var customerEmail = dto.CustomerEmail?.Trim();

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

        // Referencia determinística por intención (deriva del idempotencyKey):
        // un reintento con la misma clave corta en el check de arriba y nunca
        // reenvía la misma referencia a Wompi.
        var transaction = new Transaction(
            userId: userId,
            idempotencyKey: dto.IdempotencyKey,
            transactionReference: $"shopniu_{dto.IdempotencyKey}",
            status: TransactionStatus.PENDING
        );

        // Create orders by product
        var orders = dto.Products
        .Select(item => new Order(userId, item.ProductId, item.Quantity, transaction)).ToList();

        // get payment details
        var paymentDetails = PaymentDetails.Create(
            products: productsWithQuantity,
            deliveryInCents: 0,
            currency: dto.Currency,
            paymentMethod: dto.PaymentMethod,
            transaction: transaction
        );

        await _transactionRepository.CreateAsync(transaction);

        // Guardar los datos de pago del cliente solo si lo solicitó y tiene
        // cuenta (userId 0 = guest: nunca se guarda). Se evita duplicar el
        // registro cuando ya existe el mismo usuario + tarjeta + dirección.
        if (dto.SavePayment && userId != 0)
        {
            var existingPaymentData = await _userPaymentDataRepository
                .GetByUserIdAndLastFourAsync(userId, dto.CardLastFour);
            if (!existingPaymentData.Any(pd => pd.Matches(userId, dto.Delivery.Address, dto.CardLastFour)))
            {
                var userPaymentData = new UserPaymentData(
                                cardNumber: _cardEncryption.Encrypt(dto.CardNumber),
                                cardHolderName: dto.CardHolderName ?? "",
                                address: dto.Delivery.Address,
                                phoneNumber: dto.Delivery.Phone ?? "",
                                city: dto.Delivery.City,
                                department: dto.Delivery.Department,
                                departmentCode: dto.Delivery.DepartmentCode,
                                cityCode: dto.Delivery.CityCode,
                                lastFour: dto.CardLastFour,
                                userId: userId,
                                paymentMethod: dto.PaymentMethod,
                                expMonth: dto.ExpMonth,
                                expYear: dto.ExpYear
                            );
                await _userPaymentDataRepository.CreateAsync(userPaymentData);
            }
        }

        // El delivery nace PENDING, igual que la transacción; el webhook lo
        // actualiza según el resultado del pago.
        var delivery = new Delivery(
            address: dto.Delivery.Address,
            city: dto.Delivery.City,
            department: dto.Delivery.Department,
            departmentCode: dto.Delivery.DepartmentCode,
            cityCode: dto.Delivery.CityCode,
            userId: userId,
            transaction: transaction
        );
        await _deliveryRepository.CreateAsync(delivery);

        // Las órdenes ya vienen agrupadas por producto; se agregan al contexto
        // junto con el resto de los agregados y el SaveChangesAsync único del
        // final las persiste de forma atómica con la transacción.
        await _orderRepository.CreateRangeAsync(orders);

        // Create payment request by payment gateway
        var paymentRequest = new PaymentRequest(
            AmountInCents: paymentDetails.TotalInCents,
            Currency: dto.Currency,
            Reference: transaction.TransactionReference,
            Email: customerEmail,
            PaymentMethod: dto.PaymentMethod,
            ProviderToken: dto.ProviderToken,
            AcceptanceToken: dto.AcceptanceToken,
            AcceptancePersonalToken: dto.AcceptancePersonalToken
        );

        // TEMP DEBUG: logear qué se manda a Wompi para el error de monto mínimo.
        _logger.LogInformation(
            "PAY-DEBUG: currency={Currency} amountInCents={Amount} totalInCents={Total} products=[{Products}]",
            paymentRequest.Currency,
            paymentRequest.AmountInCents,
            paymentDetails.TotalInCents,
            string.Join(", ", productsWithQuantity.Select(p => $"{p.Product.Id}:{p.quantity}@{p.Product.Price}")));

        var paymentResponse = await _paymentGateway.CreatePayment(paymentRequest);

        // Update transaction with necesary data payment response
        transaction.UpdatePaymentResult(paymentResponse.Id, paymentResponse.Status);
        await _transactionRepository.UpdateAsync(transaction);

        await _unitOfWork.SaveChangesAsync();

        return new CreateTransactionResponse(
            TransactionId: transaction.Id,
            Reference: transaction.TransactionReference,
            Status: transaction.Status
        );
    }
}