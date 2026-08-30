using FluentValidation;

namespace Shopniu_api.Aplication.Products.UseCases.ImportProducts;

public class ImportProductsValidator : AbstractValidator<ImportProductsRequest>
{
    public ImportProductsValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Se requiere al menos un producto para importar.");

        RuleForEach(x => x.Items).SetValidator(new ImportProductItemValidator());
    }
}

public class ImportProductItemValidator : AbstractValidator<ImportProductItem>
{
    public ImportProductItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.CostPrice)
            .GreaterThan(0).WithMessage("Cost price must be greater than zero.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Image URL must be a valid absolute URL.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock must be zero or greater.");

        RuleFor(x => x.SupplierName)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");

        RuleFor(x => x.LeadTimeDays)
            .GreaterThan(0).WithMessage("Lead time days must be greater than zero.");
    }
}
