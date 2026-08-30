using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(bool includeMedia = false);
        Task<List<Product>> GetByOwnerIdAsync(int userId);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByIdsAsync(List<int> ids);
        Task<Product?> GetBySupplierAndSkuAsync(int supplierId, string supplierSku);
        Task<Product> CreateAsync(Product product);
        Task AddOwnerAsync(ProductOwner owner);
        Task<bool> IsOwnedByAsync(int productId, int userId);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}