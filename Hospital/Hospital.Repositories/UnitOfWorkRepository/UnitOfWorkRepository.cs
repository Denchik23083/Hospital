using Hospital.Db;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hospital.Repositories.UnitOfWorkRepository
{
    public class UnitOfWorkRepository(HospitalContext context) : IUnitOfWorkRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
