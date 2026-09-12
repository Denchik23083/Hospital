using Hospital.Db;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hospital.Repositories.UnitOfWorkRepository
{
    public class UnitOfWorkRepository(HospitalContext context) : IUnitOfWorkRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct)
        {
            return await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
