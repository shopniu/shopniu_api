using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Domain.Repositories
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetActiveAsync();
        Task<List<Supplier>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<Supplier> CreateAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
    }
}
