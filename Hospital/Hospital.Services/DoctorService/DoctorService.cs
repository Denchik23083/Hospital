using Hospital.Core.Models.Responce;
using Hospital.Db;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(HospitalContext context) : IDoctorService
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DoctorResponce>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _context.Doctors
                .Where(_ => _.SpecialtyId == specialtyId)
                .Select(_ => new DoctorResponce
                {
                    Id = _.Id,
                    FirstName = _.FirstName,
                    LastName = _.LastName,
                    ExperienceYears = _.ExperienceYears,
                    GenderType = _.GenderType.ToString(),
                }).ToListAsync();
        }
    }
}
