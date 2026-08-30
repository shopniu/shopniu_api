using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Suppliers.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Suppliers.UseCases.ListSupplierSyncLogs;

public class ListSupplierSyncLogsUseCase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierSyncLogRepository _syncLogRepository;
    private readonly ICurrentUserService _currentUser;

    public ListSupplierSyncLogsUseCase(
        ISupplierRepository supplierRepository,
        ISupplierSyncLogRepository syncLogRepository,
        ICurrentUserService currentUser)
    {
        _supplierRepository = supplierRepository;
        _syncLogRepository = syncLogRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<SupplierSyncLogDTO>> ExecuteAsync(int supplierId)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        _ = await _supplierRepository.GetByIdAsync(supplierId)
            ?? throw new NotFoundException("Supplier", supplierId);

        var logs = await _syncLogRepository.GetBySupplierIdAsync(supplierId);
        return logs.Select(log => new SupplierSyncLogDTO(
            log.Id,
            log.RunAt,
            log.Succeeded,
            log.Created,
            log.Updated,
            log.ErrorCount,
            log.Errors
        ));
    }
}
