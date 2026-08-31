using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Aplication.Products.Ports;
using Shopniu_api.Domain.Exceptions.Common;

namespace Shopniu_api.Aplication.Products.UseCases.ExtractProductFromUrl;

/// <summary>Extrae la información de un producto desde su URL (server-side,
/// JSON-LD/OpenGraph). NO crea nada: el front usa el resultado como preview
/// del flujo de importación. El precio extraído se trata como costo.</summary>
public class ExtractProductFromUrlUseCase
{
    private readonly IProductUrlExtractor _extractor;
    private readonly ICurrentUserService _currentUser;

    public ExtractProductFromUrlUseCase(
        IProductUrlExtractor extractor,
        ICurrentUserService currentUser)
    {
        _extractor = extractor;
        _currentUser = currentUser;
    }

    public async Task<ExtractedProductDTO> ExecuteAsync(ExtractProductFromUrlRequest dto)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var extracted = await _extractor.ExtractAsync(dto.Url)
            ?? throw new ValidationsException(
                "No se pudo extraer información del producto desde la URL.");

        return new ExtractedProductDTO(
            extracted.Name,
            extracted.ImageUrl,
            extracted.Description,
            extracted.Price,
            extracted.Brand);
    }
}
