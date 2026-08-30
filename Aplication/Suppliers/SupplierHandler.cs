using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Aplication.Suppliers.UseCases.CreateSupplier;
using Shopniu_api.Aplication.Suppliers.UseCases.ListSupplierSyncLogs;
using Shopniu_api.Aplication.Suppliers.UseCases.ListSuppliers;
using Shopniu_api.Aplication.Suppliers.UseCases.SyncSupplierCatalog;
using Shopniu_api.Aplication.Suppliers.UseCases.UpdateSupplier;
using Shopniu_api.Aplication.Suppliers.Common.DTOs;
using Shopniu_shared.Common;

namespace Shopniu_api.Aplication.Suppliers;

public class SupplierHandler
{
    private readonly ListSuppliersUseCase _listSuppliersUseCase;
    private readonly CreateSupplierUseCase _createSupplierUseCase;
    private readonly UpdateSupplierUseCase _updateSupplierUseCase;
    private readonly SyncSupplierCatalogUseCase _syncSupplierCatalogUseCase;
    private readonly ListSupplierSyncLogsUseCase _listSupplierSyncLogsUseCase;
    private readonly ICurrentUserService _currentUser;

    public SupplierHandler(
        ListSuppliersUseCase listSuppliersUseCase,
        CreateSupplierUseCase createSupplierUseCase,
        UpdateSupplierUseCase updateSupplierUseCase,
        SyncSupplierCatalogUseCase syncSupplierCatalogUseCase,
        ListSupplierSyncLogsUseCase listSupplierSyncLogsUseCase,
        ICurrentUserService currentUser)
    {
        _listSuppliersUseCase = listSuppliersUseCase;
        _createSupplierUseCase = createSupplierUseCase;
        _updateSupplierUseCase = updateSupplierUseCase;
        _syncSupplierCatalogUseCase = syncSupplierCatalogUseCase;
        _listSupplierSyncLogsUseCase = listSupplierSyncLogsUseCase;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<IEnumerable<SupplierResponseDTO>>> ListSuppliersAsync()
    {
        var result = await _listSuppliersUseCase.ExecuteAsync();
        return ApiResponse<IEnumerable<SupplierResponseDTO>>.Ok(result, "Suppliers Retrieved Successfully");
    }

    public async Task<ApiResponse<SupplierResponseDTO>> CreateSupplierAsync(SupplierRequest dto)
    {
        var result = await _createSupplierUseCase.ExecuteAsync(dto);
        return ApiResponse<SupplierResponseDTO>.Ok(result, "Supplier Created Successfully");
    }

    public async Task<ApiResponse<SupplierResponseDTO>> UpdateSupplierAsync(int id, SupplierRequest dto)
    {
        var result = await _updateSupplierUseCase.ExecuteAsync(id, dto);
        return ApiResponse<SupplierResponseDTO>.Ok(result, "Supplier Updated Successfully");
    }

    public async Task<ApiResponse<SyncSupplierCatalogResult>> SyncSupplierAsync(int id)
    {
        var result = await _syncSupplierCatalogUseCase.ExecuteAsync(id, _currentUser.UserId);
        return ApiResponse<SyncSupplierCatalogResult>.Ok(result, "Supplier catalog synced successfully");
    }

    public async Task<ApiResponse<IEnumerable<SupplierSyncLogDTO>>> GetSyncLogsAsync(int id)
    {
        var result = await _listSupplierSyncLogsUseCase.ExecuteAsync(id);
        return ApiResponse<IEnumerable<SupplierSyncLogDTO>>.Ok(result, "Supplier sync logs retrieved successfully");
    }
}
