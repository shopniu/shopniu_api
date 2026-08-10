

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;

public interface ISignaturePayload
{
    long AmountInCents { get; set; }
    CurrencyType Currency { get; set; }
    string Reference { get; set; }
}
public class WompiSignatureGenerator
{
    private readonly string _IntegrityKey;

    public WompiSignatureGenerator(IOptions<WompiSettings> Options)
    {
        _IntegrityKey = Options.Value.IntegrityKey;
    }

    public string GenerateSignature(ISignaturePayload payload)
    {
        var payloadString = $"{payload.Reference}{payload.AmountInCents}{payload.Currency}{_IntegrityKey}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(payloadString));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}