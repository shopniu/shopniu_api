using FluentValidation;

namespace Shopniu_api.Aplication.Products.UseCases.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Image URL must be a valid absolute URL.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock  must be zero or greater.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).When(x => x.CostPrice.HasValue)
            .WithMessage("Cost price must be zero or greater.");

        RuleFor(x => x.LeadTimeDays)
            .GreaterThan(0).When(x => x.LeadTimeDays.HasValue)
            .WithMessage("Lead time days must be greater than zero.");

        RuleFor(x => x.SupplierId)
            .GreaterThan(0).When(x => x.SupplierId.HasValue)
            .WithMessage("Supplier ID must be a positive integer.");

        RuleFor(x => x.SupplierName)
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");
    }
}