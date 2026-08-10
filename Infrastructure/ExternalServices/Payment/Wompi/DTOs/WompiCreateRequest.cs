using System.Text.Json.Serialization;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.DTOs;

public class WompiCreateRequest : ISignaturePayload
{
    [JsonPropertyName("amount_in_cents")]
    public long AmountInCents { get; set; }

    [JsonPropertyName("currency")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyType Currency { get; set; } = CurrencyType.COP;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("customer_email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("payment_method")]
    public WompiPaymentMethodDto PaymentMethod { get; set; } = null!;

    [JsonPropertyName("acceptance_token")]
    public string AcceptanceToken { get; set; } = string.Empty;

    [JsonPropertyName("accept_personal_auth")]
    public string AcceptancePersonalToken { get; set; } = string.Empty;

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}

public class WompiPaymentMethodDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("installments")]
    public int Installments { get; set; }
}