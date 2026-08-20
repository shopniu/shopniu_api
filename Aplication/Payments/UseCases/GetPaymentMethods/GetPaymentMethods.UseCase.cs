using Shopniu_api.Aplication.Common.Ports.CardEncryption;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Aplication.Payments.UseCases.GetPaymentMethods;

public class GetPaymentMethodsUseCase
{
    private readonly IUserPaymentDataRepository _userPaymentDataRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ICardEncryptionService _cardEncryption;

    public GetPaymentMethodsUseCase(
        IUserPaymentDataRepository userPaymentDataRepository,
        ICurrentUserService currentUser,
        ICardEncryptionService cardEncryption)
    {
        _userPaymentDataRepository = userPaymentDataRepository;
        _currentUser = currentUser;
        _cardEncryption = cardEncryption;
    }

    /// <summary>Métodos de pago guardados del usuario autenticado. Si la petición
    /// es anónima (userId 0) no hay métodos guardados que devolver.</summary>
    public async Task<IEnumerable<UserPaymentMethodResponse>> ExecuteAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            return [];
        }

        var methods = await _userPaymentDataRepository.GetByUserIdAsync(userId);
        return methods.Select(method =>
            UserPaymentMethodResponse.FromEntity(
                method,
                decryptedCardNumber: _cardEncryption.Decrypt(method.CardNumber)));
    }
}