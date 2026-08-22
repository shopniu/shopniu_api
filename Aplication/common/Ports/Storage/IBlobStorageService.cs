namespace Shopniu_api.Aplication.Common.Ports.Storage;

/// <summary>Credenciales para que el front suba un archivo directo a Blob
/// Storage (SAS de escritura con expiración corta).</summary>
public sealed record BlobUploadCredentials(
    string UploadUrl,
    string BlobPath,
    string PublicUrl);

public interface IBlobStorageService
{
    /// <summary>Genera una SAS de escritura efímera para un blob nuevo y
    /// devuelve la URL pública del mismo (para leerlo tras el upload).</summary>
    Task<BlobUploadCredentials> CreateUploadCredentialsAsync(
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<byte[]> DownloadAsync(string blobPath, CancellationToken cancellationToken = default);
    Task UploadAsync(string blobPath, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default);
    Task EnsureContainerExistsAsync(CancellationToken cancellationToken = default);

    string ResolvePublicUrl(string blobPath);

    /// <summary>Path de una variante derivada del original, ej. "2026/08/g.jpg"
    /// con suffix "web" → "2026/08/g_web.jpg".</summary>
    string BuildVariantPath(string blobPath, string suffix);
}
