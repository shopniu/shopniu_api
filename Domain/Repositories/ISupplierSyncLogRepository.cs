using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Domain.Repositories
{
    public interface ISupplierSyncLogRepository
    {
        Task CreateAsync(SupplierSyncLog log);
        Task<SupplierSyncLog?> GetLatestForSupplierAsync(int supplierId);
        Task<List<SupplierSyncLog>> GetBySupplierIdAsync(int supplierId, int limit = 20);
    }
}
