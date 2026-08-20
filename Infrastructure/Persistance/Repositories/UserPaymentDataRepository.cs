using Shopniu_api.Domain.Entities.UserPaymentDataEntity;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Shopniu_api.Infrastructure.Persistance.Repositories;

public class UserPaymentDataRepository : IUserPaymentDataRepository
{
    private readonly AppDbContext _context;

    public UserPaymentDataRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserPaymentData> CreateAsync(UserPaymentData userPaymentData)
    {
        _context.UserPaymentData.Add(userPaymentData);

        return userPaymentData;
    }

    public async Task<List<UserPaymentData>> GetByUserIdAndLastFourAsync(int userId, int lastFour)
    {
        return await _context.UserPaymentData
            .Where(upd => upd.UserId == userId && upd.LastFour == lastFour)
            .ToListAsync();
    }

    public async Task<List<UserPaymentData>> GetByUserIdAsync(int userId)
    {
        return await _context.UserPaymentData
            .Where(upd => upd.UserId == userId)
            .OrderByDescending(upd => upd.Id)
            .ToListAsync();
    }
}