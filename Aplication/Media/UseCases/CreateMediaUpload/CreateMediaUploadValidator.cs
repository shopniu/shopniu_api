using FluentValidation;

namespace Shopniu_api.Aplication.Media.UseCases.CreateMediaUpload;

public class CreateMediaUploadValidator : AbstractValidator<CreateMediaUploadRequest>
{
    public CreateMediaUploadValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.");
    }
}
