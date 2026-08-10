namespace Shopniu_api.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}