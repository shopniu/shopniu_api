using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Entities.UserPaymentDataEntity;

namespace Shopniu_api.Aplication.Payments.UseCases.GetPaymentMethods;

/// <summary>Método de pago guardado por un usuario. Nunca expone el PAN
/// (no se persiste): solo los últimos 4 dígitos y los datos de entrega.</summary>
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
    PaymentMethodType PaymentMethod
)
{
    public static UserPaymentMethodResponse FromEntity(UserPaymentData data)
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
            data.PaymentMethod
        );
    }

    public static IEnumerable<UserPaymentMethodResponse> FromEntities(IEnumerable<UserPaymentData> data)
    {
        return data.Select(FromEntity);
    }
}