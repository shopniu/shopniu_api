using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Aplication.Common.Ports.Storage;
using Shopniu_api.Aplication.Media.UseCases.CreateMediaUpload;
using Shopniu_api.Domain.Exceptions.Common;

namespace Shopniu_api.Aplication.Media;

public class CreateMediaUploadUseCase
{
    private readonly IBlobStorageService _storage;
    private readonly ICurrentUserService _currentUser;

    public CreateMediaUploadUseCase(IBlobStorageService storage, ICurrentUserService currentUser)
    {
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<MediaUploadResponse> ExecuteAsync(CreateMediaUploadRequest request, CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        // La política product.create ya valida el permiso; aquí solo se emite la
        // SAS de escritura efímera para que el front suba directo a Blob.
        var credentials = await _storage.CreateUploadCredentialsAsync(
            request.FileName,
            request.ContentType,
            cancellationToken);

        return new MediaUploadResponse(credentials.UploadUrl, credentials.BlobPath, credentials.PublicUrl);
    }
}
