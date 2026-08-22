using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Aplication.Media.UseCases.SetMainMedia;

/// <summary>Marca una media como principal dentro de su producto y sincroniza
/// Product.ImageUrl con su variante web.</summary>
public class SetMainMediaUseCase
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetMainMediaUseCase(IMediaRepository mediaRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MediaAssetResponse> ExecuteAsync(int mediaId, CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId);
        if (media == null)
        {
            throw new NotFoundException("Media", mediaId);
        }

        if (media.ProductId is not { } productId)
        {
            throw new Domain.Exceptions.BusinessRuleException("Media is not linked to a product.");
        }

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new NotFoundException("Product", productId);
        }

        var all = await _mediaRepository.GetByProductIdAsync(productId);
        foreach (var item in all)
        {
            if (item.IsMain)
            {
                item.IsMain = false;
            }
        }

        media.IsMain = true;
        product.ImageUrl = media.WebUrl;

        await _mediaRepository.UpdateAsync(media);
        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MediaAssetResponse.FromEntity(media);
    }
}
