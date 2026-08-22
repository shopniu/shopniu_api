using FluentValidation;

namespace Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;

public class ConfirmMediaUploadValidator : AbstractValidator<ConfirmMediaUploadRequest>
{
    // Los blob paths los genera el servidor con el formato yyyy/MM/guid.ext.
    private const string BlobPathPattern = "^[0-9]{4}/[0-9]{2}/[a-f0-9]{32}\\.(jpg|jpeg|png|webp)$";

    public ConfirmMediaUploadValidator()
    {
        RuleFor(x => x.BlobPath)
            .NotEmpty().WithMessage("Blob path is required.")
            .Matches(BlobPathPattern).WithMessage("Blob path is invalid.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product id must be greater than zero.")
            .When(x => x.ProductId.HasValue);
    }
}
