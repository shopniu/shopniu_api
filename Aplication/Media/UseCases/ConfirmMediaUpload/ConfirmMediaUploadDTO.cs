using Shopniu_api.Domain.Entities.MediaEntity;

namespace Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;

public sealed record ConfirmMediaUploadRequest(
    string BlobPath,
    int? ProductId,
    bool IsMain = false);

public sealed record MediaAssetResponse(
    int Id,
    string OriginalUrl,
    string WebUrl,
    string ThumbUrl,
    int? ProductId,
    bool IsMain,
    string ContentType,
    long Size,
    int Width,
    int Height)
{
    public static MediaAssetResponse FromEntity(MediaAsset media)
    {
        return new MediaAssetResponse(
            media.Id,
            media.OriginalUrl,
            media.WebUrl,
            media.ThumbUrl,
            media.ProductId,
            media.IsMain,
            media.ContentType,
            media.Size,
            media.Width,
            media.Height
        );
    }
}
