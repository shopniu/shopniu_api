using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Suppliers.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Suppliers.UseCases.ListSuppliers;

public class ListSuppliersUseCase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierSyncLogRepository _syncLogRepository;
    private readonly ICurrentUserService _currentUser;

    public ListSuppliersUseCase(
        ISupplierRepository supplierRepository,
        ISupplierSyncLogRepository syncLogRepository,
        ICurrentUserService currentUser)
    {
        _supplierRepository = supplierRepository;
        _syncLogRepository = syncLogRepository;
        _currentUser = currentUser;
    }

    /// <summary>Proveedores activos para el back-office, con el resumen de su
    /// última sincronización. La política product.create ya garantiza sesión;
    /// userId 0 indica una misconfiguración de issuer/claims y se reporta
    /// como 401.</summary>
    public async Task<IEnumerable<SupplierResponseDTO>> ExecuteAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var suppliers = await _supplierRepository.GetActiveAsync();

        return await SupplierResponseDTO.FromEntitiesAsync(suppliers, async supplier =>
        {
            var last = await _syncLogRepository.GetLatestForSupplierAsync(supplier.Id);
            return last is null
                ? null
                : new SupplierSyncSummaryDTO(last.RunAt, last.Succeeded, last.Created, last.Updated, last.ErrorCount);
        });
    }
}
