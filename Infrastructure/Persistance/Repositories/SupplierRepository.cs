using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.SupplierEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetActiveAsync()
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers.FindAsync(id);
    }

    public Task<Supplier> CreateAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        return Task.FromResult(supplier);
    }

    public Task UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        return Task.CompletedTask;
    }
}
