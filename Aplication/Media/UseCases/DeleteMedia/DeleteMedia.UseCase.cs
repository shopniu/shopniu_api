using Shopniu_api.Aplication.Common.Ports.Storage;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Aplication.Media.UseCases.DeleteMedia;

/// <summary>Elimina la media: borra los blobs (original + variantes) y el
/// registro. Si era la principal, limpia Product.ImageUrl.</summary>
public class DeleteMediaUseCase
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBlobStorageService _storage;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMediaUseCase(
        IMediaRepository mediaRepository,
        IProductRepository productRepository,
        IBlobStorageService storage,
        IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _productRepository = productRepository;
        _storage = storage;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int mediaId, CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId);
        if (media == null)
        {
            throw new NotFoundException("Media", mediaId);
        }

        await _storage.DeleteAsync(media.BlobPath, cancellationToken);
        await _storage.DeleteAsync(_storage.BuildVariantPath(media.BlobPath, "web"), cancellationToken);
        await _storage.DeleteAsync(_storage.BuildVariantPath(media.BlobPath, "thumb"), cancellationToken);

        if (media.IsMain && media.ProductId is { } productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.ImageUrl = string.Empty;
                await _productRepository.UpdateAsync(product);
            }
        }

        await _mediaRepository.DeleteAsync(media.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}
