using Hospital.Core.Models.Response;
using Hospital.Db;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.SpecialtyService
{
    public class SpecialtyService(HospitalContext context) : ISpecialtyService
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
    }
}
