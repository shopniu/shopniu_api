
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;
using Shopniu_api.Aplication.Transactions.Ports;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.DTOs;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi;

public class WompiPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly WompiSignatureGenerator _signatureGenerator;

    public WompiPaymentGateway(HttpClient httpClient, WompiSignatureGenerator signatureGenerator, IOptions<WompiSettings> wompiSettings)
    {
        _httpClient = httpClient;
        _signatureGenerator = signatureGenerator;

        _httpClient.BaseAddress = new Uri(wompiSettings.Value.ApiUrl.TrimEnd('/') + "/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", wompiSettings.Value.PrivateKey);
    }

    public async Task<PaymentResponse> CreatePayment(PaymentRequest paymentRequest)
    {
        var wompiRequest = new WompiCreateRequest
        {
            AmountInCents = (long)paymentRequest.AmountInCents,
            Currency = paymentRequest.Currency,
            Reference = paymentRequest.Reference,
            CustomerEmail = paymentRequest.Email ?? throw new BusinessRuleException("Customer email is required for Wompi payments."),
            PaymentMethod = MapPaymentMethod(paymentRequest.PaymentMethod, paymentRequest.ProviderToken),
            AcceptanceToken = paymentRequest.AcceptanceToken,
            AcceptancePersonalToken = paymentRequest.AcceptancePersonalToken
        };

        wompiRequest.Signature = _signatureGenerator.GenerateSignature(wompiRequest);

        // Contenido NO rewindable: si la conexión se cae, HttpClient NO debe
        // reintentar automáticamente un pago (reintentar reutilizaría la misma
        // referencia y Wompi respondería "La referencia ya ha sido usada").
        var json = JsonSerializer.Serialize(wompiRequest);
        using var content = new NonRewindableJsonContent(json);

        var response = await _httpClient.PostAsync("transactions", content);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new BusinessRuleException($"Wompi respondió con error: {response.StatusCode} - {errorBody}");
        }

        var wompiResponse = await response.Content.ReadFromJsonAsync<WompiCreateResponse>()
        ?? throw new BusinessRuleException("Respuesta vacía o inválida de Wompi.");

        return new PaymentResponse(wompiResponse.Data.Id, WompiStatusMapper.Map(wompiResponse.Data.Status), wompiResponse.Data.Reference);
    }

    private static WompiPaymentMethodDto MapPaymentMethod(PaymentMethodType paymentMethod, string ProviderToken) => paymentMethod switch
    {
        PaymentMethodType.CreditCard or PaymentMethodType.DebitCard => new WompiPaymentMethodDto
        {
            Type = "CARD",
            Token = ProviderToken,
            Installments = 0
        },
        _ => throw new BusinessRuleException($"Método de pago no soportado por Wompi: {paymentMethod}")
    };

    /// <summary>HttpContent sin longitud conocida y sin buffer rewindable:
    /// impide que HttpClient reintente automáticamente la petición.</summary>
    private sealed class NonRewindableJsonContent : HttpContent
    {
        private readonly byte[] _payload;

        public NonRewindableJsonContent(string json)
        {
            _payload = Encoding.UTF8.GetBytes(json);
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            return stream.WriteAsync(_payload, 0, _payload.Length);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

}