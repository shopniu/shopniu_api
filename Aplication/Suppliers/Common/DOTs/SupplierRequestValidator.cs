using FluentValidation;

namespace Shopniu_api.Aplication.Suppliers.Common.DTOs;

public class SupplierRequestValidator : AbstractValidator<SupplierRequest>
{
    public SupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Region)
            .MaximumLength(100).WithMessage("Region must not exceed 100 characters.");

        RuleFor(x => x.DefaultShipping)
            .GreaterThanOrEqualTo(0).WithMessage("Default shipping must be zero or greater.");

        RuleFor(x => x.DefaultLeadTimeDays)
            .GreaterThan(0).WithMessage("Default lead time must be greater than zero.");
    }
}
