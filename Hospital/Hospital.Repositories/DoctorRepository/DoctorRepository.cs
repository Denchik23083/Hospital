using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.DoctorRepository
{
    public class DoctorRepository(HospitalContext context) : IDoctorRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _context.Doctors
                .Where(_ => _.SpecialtyId == specialtyId)
                .Select(_ => new DoctorResponse
                {
                    Id = _.Id,
                    FirstName = _.FirstName,
                    LastName = _.LastName,
                    ExperienceYears = _.ExperienceYears,
                    GenderType = _.GenderType.ToString(),
                }).ToListAsync();
        }

        public async Task<Doctor?> GetDoctorAsync(int userId)
        {
            return await _context.Doctors
                .FirstOrDefaultAsync(_ => _.UserId == userId);
        }
    }
}
