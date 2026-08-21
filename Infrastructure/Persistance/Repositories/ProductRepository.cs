

using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class ProductsRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductsRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }
    public async Task<List<Product>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
    }
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>Productos que el usuario tiene (ProductOwners), sin importar
    /// quién los creó originalmente.</summary>
    public async Task<List<Product>> GetByOwnerIdAsync(int userId)
    {
        return await _context.ProductOwners
            .Where(po => po.UserId == userId)
            .Select(po => po.Product)
            .ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);

        return product;
    }

    public async Task AddOwnerAsync(ProductOwner owner)
    {
        _context.ProductOwners.Add(owner);

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);

        }
    }




    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);

    }
}