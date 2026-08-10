using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Domain.Entities.PaymentDetailsEntity;

public enum PaymentMethodType
{
    CreditCard,
    DebitCard
}

public enum CurrencyType
{
    USD,
    EUR,
    COP
}

public record ProductWithQuantity(
    int quantity,
    Product Product
);

public class PaymentDetails : BaseEntity
{
    public decimal AmountInCents { get; private set; }
    public decimal TaxInCents { get; private set; }
    public decimal DeliveryInCents { get; private set; }
    public decimal TotalInCents { get; private set; }
    public CurrencyType Currency { get; private set; }
    public PaymentMethodType PaymentMethod { get; private set; }
    public int TransactionId { get; private set; }
    public Transaction Transaction { get; private set; } = null!;

    private PaymentDetails() { } // EF Core

    public static PaymentDetails Create(
        IEnumerable<ProductWithQuantity> products,
        int deliveryInCents,
        CurrencyType currency,
        PaymentMethodType paymentMethod,
        Transaction transaction,
        decimal taxRate = 0.19m)  // ej. IVA Colombia 19%, ajusta según tu caso
    {
        var amountInCents = products.Sum(p => p.quantity * p.Product.Price);
        if (amountInCents <= 0)
            throw new BusinessRuleException("El monto debe ser mayor a cero.");

        if (deliveryInCents < 0)
            throw new BusinessRuleException("El costo de envío no puede ser negativo.");

        // No need to check enum for null or whitespace

        var taxInCents = (int)Math.Round(amountInCents * taxRate);
        var totalInCents = amountInCents + deliveryInCents;

        return new PaymentDetails
        {
            AmountInCents = amountInCents,
            TaxInCents = taxInCents,
            DeliveryInCents = deliveryInCents,
            TotalInCents = totalInCents,
            Currency = currency,
            PaymentMethod = paymentMethod,
            Transaction = transaction,
        };
    }
}