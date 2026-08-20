using Shopniu_api.Domain.Entities.UserPaymentDataEntity;

namespace Shopniu_api.Domain.Repositories;

public interface IUserPaymentDataRepository
{
    Task<UserPaymentData> CreateAsync(UserPaymentData userPaymentData);
    /// <summary>Candidatos al dedupe: registros del usuario con la misma tarjeta
    /// (últimos 4 dígitos). La decisión final la toma la entidad con Matches().</summary>
    Task<List<UserPaymentData>> GetByUserIdAndLastFourAsync(int userId, int lastFour);
    /// <summary>Métodos de pago guardados del usuario, más recientes primero.</summary>
    Task<List<UserPaymentData>> GetByUserIdAsync(int userId);
}