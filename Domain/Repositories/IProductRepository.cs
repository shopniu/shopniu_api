using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetByOwnerIdAsync(int userId);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByIdsAsync(List<int> ids);
        Task<Product> CreateAsync(Product product);
        Task AddOwnerAsync(ProductOwner owner);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}