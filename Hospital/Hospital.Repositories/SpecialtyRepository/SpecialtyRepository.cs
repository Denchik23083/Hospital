using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.SpecialtyRepository
{
    public class SpecialtyRepository(HospitalContext context) : ISpecialtyRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync(CancellationToken ct)
        {
            return await _context.Specialties
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<decimal?> GetSpecialtyPriceAsync(int specialtyId, CancellationToken ct)
        {
            return await _context.Specialties
                .Where(_ => _.Id == specialtyId)
                .Select(_ => (decimal?)_.Price)
                .FirstOrDefaultAsync(ct);
        }
    }
}
