
using Hospital.Db;

namespace Hospital.Repositories.UnitOfWorkRepository
{
    public class UnitOfWorkRepository(HospitalContext context) : IUnitOfWorkRepository
    {
        private readonly HospitalContext _context = context;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
