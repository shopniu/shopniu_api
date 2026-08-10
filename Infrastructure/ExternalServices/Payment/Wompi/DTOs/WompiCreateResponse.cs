using System.Text.Json.Serialization;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.DTOs;

public class WompiCreateResponse
{
    [JsonPropertyName("data")]
    public WompiTransactionDto Data { get; set; } = null!;
}

public class WompiTransactionDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
}