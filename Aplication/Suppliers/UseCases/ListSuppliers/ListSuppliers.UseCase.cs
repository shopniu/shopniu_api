using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Suppliers.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Suppliers.UseCases.ListSuppliers;

public class ListSuppliersUseCase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICurrentUserService _currentUser;

    public ListSuppliersUseCase(ISupplierRepository supplierRepository, ICurrentUserService currentUser)
    {
        _supplierRepository = supplierRepository;
        _currentUser = currentUser;
    }

    /// <summary>Proveedores activos para el back-office. La política
    /// product.create ya garantiza sesión; userId 0 indica una
    /// misconfiguración de issuer/claims y se reporta como 401.</summary>
    public async Task<IEnumerable<SupplierResponseDTO>> ExecuteAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var suppliers = await _supplierRepository.GetActiveAsync();
        return SupplierResponseDTO.FromEntities(suppliers);
    }
}
