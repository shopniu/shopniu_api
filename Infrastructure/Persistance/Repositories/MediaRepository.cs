using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.MediaEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly AppDbContext _context;

    public MediaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MediaAsset?> GetByIdAsync(int id)
    {
        return await _context.MediaAssets.FindAsync(id);
    }

    public async Task<List<MediaAsset>> GetByProductIdAsync(int productId)
    {
        return await _context.MediaAssets
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.IsMain)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<MediaAsset> CreateAsync(MediaAsset media)
    {
        _context.MediaAssets.Add(media);

        return media;
    }

    public Task UpdateAsync(MediaAsset media)
    {
        _context.MediaAssets.Update(media);

        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var media = await _context.MediaAssets.FindAsync(id);
        if (media != null)
        {
            _context.MediaAssets.Remove(media);
        }
    }
}
