using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.DoctorRepository
{
    public class DoctorRepository(HospitalContext context) : IDoctorRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DoctorWithUserResponse>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .Include(_ => _.User)
                .Include(_ => _.Specialty)
                .Select(_ => new DoctorWithUserResponse
                {
                    Id = _.Id,
                    FirstName = _.FirstName,
                    LastName = _.LastName,
                    ExperienceYears = _.ExperienceYears,
                    GenderType = _.GenderType,
                    WorkDayStart = _.WorkDayStart,
                    WorkDayEnd = _.WorkDayEnd,
                    User = new UserResponse
                    {
                        Email = _.User!.Email,
                        Money = _.User!.Money
                    },
                    Specialty = new SpecialtyResponse
                    {
                        Id = _.Specialty!.Id,
                        Name = _.Specialty!.Name,
                        Price = _.Specialty!.Price
                    }
                }).ToListAsync();
        }

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
                    GenderType = _.GenderType,
                }).ToListAsync();
        }

        public async Task<Doctor?> GetDoctorAsync(int id)
        {
            return await _context.Doctors
                .Include(_ => _.User)
                .FirstOrDefaultAsync(_ => _.Id == id);
        }

        public async Task<Doctor?> GetDoctorByUserAsync(int userId)
        {
            return await _context.Doctors
                .Include(_ => _.User)
                .Include(_ => _.Specialty)
                .FirstOrDefaultAsync(_ => _.UserId == userId);
        }

        public async Task CreateDoctorAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }

        public Task DeleteDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);

            return Task.CompletedTask;
        }
    }
}
