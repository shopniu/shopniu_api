using Shopniu_api.Domain.Entities.MediaEntity;

namespace Shopniu_api.Domain.Repositories
{
    public interface IMediaRepository
    {
        Task<MediaAsset?> GetByIdAsync(int id);
        Task<List<MediaAsset>> GetByProductIdAsync(int productId);
        Task<MediaAsset> CreateAsync(MediaAsset media);
        Task UpdateAsync(MediaAsset media);
        Task DeleteAsync(int id);
    }
}
