using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.CreateMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.DeleteMedia;
using Shopniu_api.Aplication.Media.UseCases.LinkMedia;
using Shopniu_api.Aplication.Media.UseCases.SetMainMedia;
using Shopniu_shared.Common;

namespace Shopniu_api.Aplication.Media;

public class MediaHandler
{
    private readonly CreateMediaUploadUseCase _createMediaUploadUseCase;
    private readonly ConfirmMediaUploadUseCase _confirmMediaUploadUseCase;
    private readonly SetMainMediaUseCase _setMainMediaUseCase;
    private readonly LinkMediaUseCase _linkMediaUseCase;
    private readonly DeleteMediaUseCase _deleteMediaUseCase;

    public MediaHandler(
        CreateMediaUploadUseCase createMediaUploadUseCase,
        ConfirmMediaUploadUseCase confirmMediaUploadUseCase,
        SetMainMediaUseCase setMainMediaUseCase,
        LinkMediaUseCase linkMediaUseCase,
        DeleteMediaUseCase deleteMediaUseCase)
    {
        _createMediaUploadUseCase = createMediaUploadUseCase;
        _confirmMediaUploadUseCase = confirmMediaUploadUseCase;
        _setMainMediaUseCase = setMainMediaUseCase;
        _linkMediaUseCase = linkMediaUseCase;
        _deleteMediaUseCase = deleteMediaUseCase;
    }

    public async Task<ApiResponse<MediaUploadResponse>> CreateUploadUrlAsync(CreateMediaUploadRequest dto, CancellationToken cancellationToken)
    {
        var result = await _createMediaUploadUseCase.ExecuteAsync(dto, cancellationToken);
        return ApiResponse<MediaUploadResponse>.Ok(result, "Upload URL created successfully");
    }

    public async Task<ApiResponse<MediaAssetResponse>> ConfirmMediaAsync(ConfirmMediaUploadRequest dto, CancellationToken cancellationToken)
    {
        var result = await _confirmMediaUploadUseCase.ExecuteAsync(dto, cancellationToken);
        return ApiResponse<MediaAssetResponse>.Ok(result, "Media uploaded successfully");
    }

    public async Task<ApiResponse<MediaAssetResponse>> SetMainAsync(int mediaId, CancellationToken cancellationToken)
    {
        var result = await _setMainMediaUseCase.ExecuteAsync(mediaId, cancellationToken);
        return ApiResponse<MediaAssetResponse>.Ok(result, "Main media updated successfully");
    }

    public async Task<ApiResponse<List<MediaAssetResponse>>> LinkMediaAsync(LinkMediaRequest dto, CancellationToken cancellationToken)
    {
        var result = await _linkMediaUseCase.ExecuteAsync(dto, cancellationToken);
        return ApiResponse<List<MediaAssetResponse>>.Ok(result, "Media linked successfully");
    }

    public async Task<ApiResponse<bool>> DeleteMediaAsync(int mediaId, CancellationToken cancellationToken)
    {
        await _deleteMediaUseCase.ExecuteAsync(mediaId, cancellationToken);
        return ApiResponse<bool>.Ok(true, "Media deleted successfully");
    }
}
