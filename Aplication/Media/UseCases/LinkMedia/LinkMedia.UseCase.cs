using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.LinkMedia;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Aplication.Media;

/// <summary>Vincula media huérfana (subida antes de crear el producto) a un
/// producto existente. La primera media vinculada pasa a ser la principal.</summary>
public class LinkMediaUseCase
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LinkMediaUseCase(IMediaRepository mediaRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MediaAssetResponse>> ExecuteAsync(LinkMediaRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new NotFoundException("Product", request.ProductId);
        }

        var existing = await _mediaRepository.GetByProductIdAsync(product.Id);
        var firstIsMain = existing.Count == 0;

        var linked = new List<MediaAssetResponse>();
        foreach (var mediaId in request.MediaIds)
        {
            var media = await _mediaRepository.GetByIdAsync(mediaId);
            if (media == null)
            {
                throw new NotFoundException("Media", mediaId);
            }

            media.ProductId = product.Id;
            if (firstIsMain)
            {
                media.IsMain = true;
                product.ImageUrl = media.WebUrl;
                firstIsMain = false;
            }

            linked.Add(MediaAssetResponse.FromEntity(media));
        }

        await _unitOfWork.SaveChangesAsync();
        return linked;
    }
}
