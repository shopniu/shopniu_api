using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Shopniu_api.Aplication.Common.Ports.Storage;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Domain.Exceptions.Common;

namespace Shopniu_api.Infrastructure.ExternalServices.Storage;

/// <summary>Persistencia de media en Azure Blob Storage. En producción se
/// autentica con Managed Identity (user delegation SAS); en desarrollo con la
/// connection string de Azurite.</summary>
public class AzureBlobStorageService : IBlobStorageService
{
    private readonly StorageSettings _settings;
    private readonly BlobServiceClient _serviceClient;
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageService(IOptions<StorageSettings> options)
    {
        _settings = options.Value;
        _serviceClient = BuildServiceClient(_settings);
        _containerClient = _serviceClient.GetBlobContainerClient(_settings.ContainerName);
    }

    private static BlobServiceClient BuildServiceClient(StorageSettings settings)
    {
        if (settings.UseConnectionString && !string.IsNullOrWhiteSpace(settings.ConnectionString))
        {
            return new BlobServiceClient(settings.ConnectionString);
        }

        var credential = BuildCredential(settings);
        var accountUri = new Uri($"https://{settings.AccountName}.blob.core.windows.net");
        return new BlobServiceClient(accountUri, credential);
    }

    private static TokenCredential BuildCredential(StorageSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.ManagedIdentityClientId))
        {
            return new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(settings.ManagedIdentityClientId));
        }

        return new DefaultAzureCredential();
    }

    public string ResolvePublicUrl(string blobPath)
        => $"{_settings.PublicBaseUrl.TrimEnd('/')}/{_settings.ContainerName}/{blobPath}";

    public string BuildVariantPath(string blobPath, string suffix)
    {
        var dot = blobPath.LastIndexOf('.');
        var basePath = dot > 0 ? blobPath[..dot] : blobPath;
        return $"{basePath}_{suffix}.jpg";
    }

    public async Task EnsureContainerExistsAsync(CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
    }

    public async Task<BlobUploadCredentials> CreateUploadCredentialsAsync(
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationsException($"Content type '{contentType}' is not allowed.");
        }

        var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) ||
            !extension.Equals("jpg", StringComparison.OrdinalIgnoreCase) &&
            !extension.Equals("jpeg", StringComparison.OrdinalIgnoreCase) &&
            !extension.Equals("png", StringComparison.OrdinalIgnoreCase) &&
            !extension.Equals("webp", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationsException($"File extension '.{extension}' is not allowed.");
        }

        var now = DateTime.UtcNow;
        var blobPath = $"{now:yyyy}/{now:MM}/{Guid.NewGuid():N}.{extension}";
        var blobClient = _containerClient.GetBlobClient(blobPath);
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(_settings.SasDurationMinutes);

        var builder = new BlobSasBuilder(BlobSasPermissions.Create | BlobSasPermissions.Write, expiresOn)
        {
            BlobContainerName = _settings.ContainerName,
            BlobName = blobPath,
            ContentType = contentType
        };

        Uri sasUri;
        if (_settings.UseConnectionString)
        {
            sasUri = blobClient.GenerateSasUri(builder);
        }
        else
        {
            var userKey = await _serviceClient.GetUserDelegationKeyAsync(
                DateTimeOffset.UtcNow.AddMinutes(-5),
                expiresOn,
                cancellationToken);
            sasUri = blobClient.GenerateUserDelegationSasUri(builder, userKey);
        }

        return new BlobUploadCredentials(sasUri.ToString(), blobPath, ResolvePublicUrl(blobPath));
    }

    public async Task<byte[]> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobPath);
        await using var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream, cancellationToken);
        return stream.ToArray();
    }

    public async Task UploadAsync(string blobPath, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobPath);
        await blobClient.UploadAsync(
            content,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobPath);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobPath);
        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }
}
