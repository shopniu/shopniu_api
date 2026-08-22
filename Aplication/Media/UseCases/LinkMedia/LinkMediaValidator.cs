using FluentValidation;

namespace Shopniu_api.Aplication.Media.UseCases.LinkMedia;

public class LinkMediaValidator : AbstractValidator<LinkMediaRequest>
{
    public LinkMediaValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product id must be greater than zero.");

        RuleFor(x => x.MediaIds)
            .NotNull().WithMessage("Media ids are required.")
            .NotEmpty().WithMessage("At least one media id is required.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Media ids must be unique.")
            .Must(ids => ids.All(id => id > 0)).WithMessage("Media ids must be greater than zero.");
    }
}
