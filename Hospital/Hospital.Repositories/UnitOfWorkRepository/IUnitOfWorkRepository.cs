using Microsoft.EntityFrameworkCore.Storage;

namespace Hospital.Repositories.UnitOfWorkRepository
{
    public interface IUnitOfWorkRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task SaveChangesAsync();
    }
}