using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.SupplierEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class SupplierSyncLogRepository : ISupplierSyncLogRepository
{
    private readonly AppDbContext _context;

    public SupplierSyncLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(SupplierSyncLog log)
    {
        _context.SupplierSyncLogs.Add(log);
        return Task.CompletedTask;
    }

    public async Task<SupplierSyncLog?> GetLatestForSupplierAsync(int supplierId)
    {
        return await _context.SupplierSyncLogs
            .Where(l => l.SupplierId == supplierId)
            .OrderByDescending(l => l.RunAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<SupplierSyncLog>> GetBySupplierIdAsync(int supplierId, int limit = 20)
    {
        return await _context.SupplierSyncLogs
            .Where(l => l.SupplierId == supplierId)
            .OrderByDescending(l => l.RunAt)
            .Take(limit)
            .ToListAsync();
    }
}
