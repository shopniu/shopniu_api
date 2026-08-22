using Microsoft.Extensions.Options;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Aplication.Common.Ports.Storage;
using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Domain.Entities.MediaEntity;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.ExternalServices.Storage;
using SkiaSharp;

namespace Shopniu_api.Aplication.Media;

public class ConfirmMediaUploadUseCase
{
    private const int WebMaxSize = 1280;
    private const int ThumbMaxSize = 320;

    private readonly IBlobStorageService _storage;
    private readonly IMediaRepository _mediaRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly StorageSettings _storageSettings;

    public ConfirmMediaUploadUseCase(
        IBlobStorageService storage,
        IMediaRepository mediaRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IOptions<StorageSettings> storageSettings)
    {
        _storage = storage;
        _mediaRepository = mediaRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _storageSettings = storageSettings.Value;
    }

    public async Task<MediaAssetResponse> ExecuteAsync(ConfirmMediaUploadRequest request, CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        if (!await _storage.ExistsAsync(request.BlobPath, cancellationToken))
        {
            throw new NotFoundException("Media", request.BlobPath);
        }

        var originalBytes = await _storage.DownloadAsync(request.BlobPath, cancellationToken);
        if (originalBytes.Length > _storageSettings.MaxSizeBytes)
        {
            throw new BusinessRuleException(
                $"File exceeds the maximum allowed size of {_storageSettings.MaxSizeBytes} bytes.");
        }

        var imageInfo = DecodeImage(originalBytes, request.BlobPath);
        var imageWidth = imageInfo.Bitmap.Width;
        var imageHeight = imageInfo.Bitmap.Height;

        var webPath = _storage.BuildVariantPath(request.BlobPath, "web");
        var thumbPath = _storage.BuildVariantPath(request.BlobPath, "thumb");

        try
        {
            await UploadVariantAsync(webPath, imageInfo.Bitmap, WebMaxSize, 80, cancellationToken);
            await UploadVariantAsync(thumbPath, imageInfo.Bitmap, ThumbMaxSize, 75, cancellationToken);
        }
        finally
        {
            imageInfo.Bitmap.Dispose();
        }

        var product = request.ProductId is { } productId
            ? await _productRepository.GetByIdAsync(productId)
            : null;
        if (request.ProductId.HasValue && product == null)
        {
            throw new NotFoundException("Product", request.ProductId!.Value);
        }

        // Primera imagen de un producto → se vuelve la principal automáticamente.
        var existing = product != null
            ? await _mediaRepository.GetByProductIdAsync(product.Id)
            : new List<MediaAsset>();
        var isMain = request.IsMain || (product != null && existing.Count == 0);

        var media = new MediaAsset
        {
            ProductId = product?.Id,
            IsMain = isMain,
            BlobPath = request.BlobPath,
            OriginalUrl = _storage.ResolvePublicUrl(request.BlobPath),
            WebUrl = _storage.ResolvePublicUrl(webPath),
            ThumbUrl = _storage.ResolvePublicUrl(thumbPath),
            ContentType = imageInfo.MimeType,
            Size = originalBytes.Length,
            Width = imageWidth,
            Height = imageHeight,
            UploadedBy = userId
        };

        if (isMain && product != null)
        {
            foreach (var item in existing)
            {
                if (item.IsMain)
                {
                    item.IsMain = false;
                }
            }

            product.ImageUrl = media.WebUrl;
            await _productRepository.UpdateAsync(product);
        }

        await _mediaRepository.CreateAsync(media);
        await _unitOfWork.SaveChangesAsync();

        return MediaAssetResponse.FromEntity(media);
    }

    private async Task UploadVariantAsync(
        string blobPath,
        SKBitmap source,
        int maxSize,
        int quality,
        CancellationToken cancellationToken)
    {
        var bytes = EncodeVariant(source, maxSize, quality);
        using var stream = new MemoryStream(bytes);
        await _storage.UploadAsync(blobPath, stream, "image/jpeg", cancellationToken);
    }

    private static (SKBitmap Bitmap, string MimeType) DecodeImage(byte[] content, string blobPath)
    {
        using var data = SKData.CreateCopy(content);
        using var codec = SKCodec.Create(data);
        if (codec == null)
        {
            throw new ValidationsException($"File '{blobPath}' is not a supported image.");
        }

        var decoded = SKBitmap.Decode(codec);
        if (decoded == null)
        {
            throw new ValidationsException($"File '{blobPath}' contains invalid image data.");
        }

        var bitmap = NormalizeOrientation(decoded, codec.EncodedOrigin);

        // Si se generó un bitmap nuevo (imagen orientada), el original ya no
        // hace falta; si se devuelve el mismo, el caller se encarga de
        // disponerlo (no liberarlo acá: se usaría un bitmap descartado).
        if (!ReferenceEquals(bitmap, decoded))
        {
            decoded.Dispose();
        }

        return (bitmap, MapMimeType(codec.EncodedFormat));
    }

    private static SKBitmap NormalizeOrientation(SKBitmap source, SKEncodedOrigin origin)
    {
        if (origin == SKEncodedOrigin.TopLeft)
        {
            return source;
        }

        var swap = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var outWidth = swap ? source.Height : source.Width;
        var outHeight = swap ? source.Width : source.Height;

        var result = new SKBitmap(new SKImageInfo(outWidth, outHeight));
        using var canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);
        canvas.Translate(outWidth / 2f, outHeight / 2f);

        var rotation = origin switch
        {
            SKEncodedOrigin.BottomRight => 180f,
            SKEncodedOrigin.LeftTop or SKEncodedOrigin.LeftBottom => 270f,
            _ => 90f
        };
        canvas.RotateDegrees(rotation);

        var flipHorizontal = origin is SKEncodedOrigin.TopRight or SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightBottom;
        var flipVertical = origin is SKEncodedOrigin.BottomLeft or SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightBottom;
        if (flipHorizontal || flipVertical)
        {
            canvas.Scale(flipHorizontal ? -1 : 1, flipVertical ? -1 : 1);
        }

        canvas.DrawBitmap(source, new SKRect(-source.Width / 2f, -source.Height / 2f, source.Width / 2f, source.Height / 2f),
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
        canvas.Flush();
        return result;
    }

    private static byte[] EncodeVariant(SKBitmap source, int maxSize, int quality)
    {
        var scale = Math.Min(1f, (float)maxSize / Math.Max(source.Width, source.Height));
        var width = Math.Max(1, (int)Math.Round(source.Width * scale));
        var height = Math.Max(1, (int)Math.Round(source.Height * scale));

        using var resized = source.Resize(new SKImageInfo(width, height),
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)) ?? source;
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
        return data.ToArray();
    }

    private static string MapMimeType(SKEncodedImageFormat format)
    {
        return format switch
        {
            SKEncodedImageFormat.Png => "image/png",
            SKEncodedImageFormat.Webp => "image/webp",
            _ => "image/jpeg"
        };
    }
}
