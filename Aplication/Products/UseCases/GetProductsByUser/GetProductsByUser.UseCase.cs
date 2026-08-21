using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Products.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Products.UseCases.GetProductsByUser;

public class GetProductsByUserUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUser;

    public GetProductsByUserUseCase(IProductRepository productRepository, ICurrentUserService currentUser)
    {
        _productRepository = productRepository;
        _currentUser = currentUser;
    }

    /// <summary>Productos que el usuario autenticado tiene (ProductOwners).
    /// La política product.create ya garantiza sesión; userId 0 indica una
    /// misconfiguración de issuer/claims y se reporta como 401.</summary>
    public async Task<IEnumerable<ProductResponseDTO>> ExecuteAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var products = await _productRepository.GetByOwnerIdAsync(userId);
        return ProductResponseDTO.FromEntities(products);
    }
}