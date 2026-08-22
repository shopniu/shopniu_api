namespace Shopniu_api.Aplication.Media.UseCases.CreateMediaUpload;

public sealed record CreateMediaUploadRequest(
    string FileName,
    string ContentType);

public sealed record MediaUploadResponse(
    string UploadUrl,
    string BlobPath,
    string PublicUrl);
