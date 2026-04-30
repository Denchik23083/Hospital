using Hospital.Core.Models.Response;
using Hospital.Db;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.SpecialtyRepository
{
    public class SpecialtyRepository(HospitalContext context) : ISpecialtyRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<SpecialtyResponse>> GetAllSpecialtiesAsync()
        {
            return await _context.Specialties
                .Select(_ => new SpecialtyResponse
                {
                    Id = _.Id,
                    Name = _.Name,
                    Price = _.Price
                }).ToListAsync();
        }

        public async Task<decimal> GetSpecialtyPriceAsync(int specialtyId)
        {
            return await _context.Specialties
                .Where(_ => _.Id == specialtyId)
                .Select(_ => _.Price)
                .FirstOrDefaultAsync();
        }
    }
}
