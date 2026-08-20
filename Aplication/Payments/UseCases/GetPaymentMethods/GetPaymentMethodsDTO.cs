using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Entities.UserPaymentDataEntity;

namespace Shopniu_api.Aplication.Payments.UseCases.GetPaymentMethods;

/// <summary>Método de pago guardado por un usuario. Expone los últimos 4
/// dígitos, el titular, los datos de entrega y —solo al dueño autenticado—
/// el PAN descifrado y el vencimiento, para que el navegador pueda
/// re-tokenizar la tarjeta con solo el CVC en la próxima compra.</summary>
public record UserPaymentMethodResponse(
    int Id,
    int LastFour,
    string CardHolderName,
    string? Address,
    string? PhoneNumber,
    string? City,
    string? Department,
    string? DepartmentCode,
    string? CityCode,
    PaymentMethodType PaymentMethod,
    string? CardNumber = null,
    string? ExpMonth = null,
    string? ExpYear = null
)
{
    public static UserPaymentMethodResponse FromEntity(UserPaymentData data, string? decryptedCardNumber = null)
    {
        return new UserPaymentMethodResponse(
            data.Id,
            data.LastFour,
            data.CardHolderName,
            data.Address,
            data.PhoneNumber,
            data.City,
            data.Department,
            data.DepartmentCode,
            data.CityCode,
            data.PaymentMethod,
            decryptedCardNumber,
            data.ExpMonth,
            data.ExpYear
        );
    }
}