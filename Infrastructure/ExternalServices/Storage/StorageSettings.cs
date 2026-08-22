// Infrastructure/ExternalServices/Storage/StorageSettings.cs
namespace Shopniu_api.Infrastructure.ExternalServices.Storage;

public class StorageSettings
{
    public const string SectionName = "Storage";

    public string AccountName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "media";
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>Desarrollo local: cuando es true se usa la connection string
    /// (Azurite) en lugar de Managed Identity.</summary>
    public bool UseConnectionString { get; set; }
    public string? ConnectionString { get; set; }

    /// <summary>Client ID de la Managed Identity asociada a la container app
    /// en producción. Vacío → DefaultAzureCredential.</summary>
    public string? ManagedIdentityClientId { get; set; }

    public int SasDurationMinutes { get; set; } = 10;
    public long MaxSizeBytes { get; set; } = 5 * 1024 * 1024;
    public string[] AllowedContentTypes { get; set; } = { "image/jpeg", "image/png", "image/webp" };
}
