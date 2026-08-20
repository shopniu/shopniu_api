using FluentValidation;

namespace Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;

/// <summary>Valida el DTO de creación de transacción. El email del cliente no
/// se valida acá: llega desde el front ya validado (zod) y, si falta, el
/// error lo devuelve la capa previa/gateway.</summary>
public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Payment method is invalid.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Currency is invalid.");

        RuleFor(x => x.ProviderToken)
            .NotEmpty().WithMessage("Provider token is required.");

        RuleFor(x => x.AcceptanceToken)
            .NotEmpty().WithMessage("Acceptance token is required.");

        RuleFor(x => x.AcceptancePersonalToken)
            .NotEmpty().WithMessage("Personal data acceptance token is required.");

        RuleFor(x => x.CardHolderName)
            .NotEmpty().WithMessage("Card holder name is required.");

        RuleFor(x => x.CardLastFour)
            .InclusiveBetween(0, 9999).WithMessage("Card last four must be between 0 and 9999.");

        RuleFor(x => x.Delivery.Address)
            .NotEmpty().WithMessage("Delivery address is required.");

        RuleFor(x => x.Delivery.Department)
            .NotEmpty().WithMessage("Delivery department is required.");

        RuleFor(x => x.Delivery.DepartmentCode)
            .NotEmpty().WithMessage("Delivery department code is required.");

        RuleFor(x => x.Delivery.City)
            .NotEmpty().WithMessage("Delivery city is required.");

        RuleFor(x => x.Delivery.CityCode)
            .NotEmpty().WithMessage("Delivery city code is required.");

        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("At least one product is required.");

        RuleForEach(x => x.Products).ChildRules(product =>
        {
            product.RuleFor(p => p.ProductId)
                .GreaterThan(0).WithMessage("Product id must be greater than zero.");

            product.RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}