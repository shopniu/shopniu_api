using System.Text.Json.Serialization;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.DTOs;

public class WompiWebhookPayload
{
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public WompiWebhookData Data { get; set; } = null!;

    [JsonPropertyName("sent_at")]
    public string SentAt { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("signature")]
    public WompiWebhookSignature Signature { get; set; } = null!;

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = string.Empty;
}

public class WompiWebhookData
{
    [JsonPropertyName("transaction")]
    public WompiWebhookTransaction Transaction { get; set; } = null!;
}

public class WompiWebhookTransaction
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("amount_in_cents")]
    public int AmountInCents { get; set; }

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class WompiWebhookSignature
{
    [JsonPropertyName("properties")]
    public List<string> Properties { get; set; } = new();

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = string.Empty;
}