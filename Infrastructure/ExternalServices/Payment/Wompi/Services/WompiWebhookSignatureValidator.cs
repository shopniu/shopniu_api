using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.DTOs;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;

public class WompiWebhookSignatureValidation
{
    private readonly string _eventsKey;

    public WompiWebhookSignatureValidation(IOptions<WompiSettings> options)
    {
        _eventsKey = options.Value.EventsKey;
    }

    public bool IsValid(WompiWebhookPayload payload)
    {
        var rawString = $"{payload.Data.Transaction.Id}{payload.Data.Transaction.Status}{payload.Data.Transaction.AmountInCents}{payload.Timestamp}{_eventsKey}";
        var expectedChecksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawString))).ToLowerInvariant();
        return expectedChecksum == payload.Signature.Checksum;
    }
}