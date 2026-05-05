using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.PatientRepository
{
    public class PatientRepository(HospitalContext context) : IPatientRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<PatientWithUserResponse>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .Include(_ => _.User)
                .Select(_ => new PatientWithUserResponse
                {
                    Id = _.Id,
                    FirstName = _.FirstName,
                    LastName = _.LastName,
                    GenderType = _.GenderType,
                    BirthDate = _.BirthDate,
                    Phone = _.Phone,
                    User = new UserResponse
                    {
                        Email = _.User!.Email,
                        Money = _.User!.Money
                    }
                }).ToListAsync();
        }

        public async Task<Patient?> GetPatientAsync(int id)
        {
            return await _context.Patients
                .Include(_ => _.User)
                .FirstOrDefaultAsync(_ => _.Id == id);
        }

        public async Task<Patient?> GetPatientByUserAsync(int userId)
        {
            return await _context.Patients
                .Include(_ => _.User)
                .FirstOrDefaultAsync(_ => _.UserId == userId);
        }

        public async Task<decimal> GetPatientBalanceAsync(int userId)
        {
            return await _context.Users
                .Where(_ => _.Id == userId)
                .Select(_ => _.Money)
                .FirstOrDefaultAsync();
        }

        public Task DeletePatientAsync(Patient patient)
        {
            _context.Patients.Remove(patient);

            return Task.CompletedTask;
        }
    }
}
