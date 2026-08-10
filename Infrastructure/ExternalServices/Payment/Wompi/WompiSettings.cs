// Infrastructure/Adapters/Payment/Wompi/WompiSettings.cs
namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi;

public class WompiSettings
{
    public const string SectionName = "Wompi";

    public string IntegrityKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string EventsKey { get; set; } = string.Empty;
}