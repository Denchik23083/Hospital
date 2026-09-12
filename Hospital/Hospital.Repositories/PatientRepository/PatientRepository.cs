using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.PatientRepository
{
    public class PatientRepository(HospitalContext context) : IPatientRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(CancellationToken ct)
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(_ => _.User)
                .ToListAsync(ct);
        }

        public async Task<Patient?> GetPatientAsync(int id, CancellationToken ct)
        {
            return await _context.Patients
                .Include(_ => _.User)
                .FirstOrDefaultAsync(_ => _.Id == id, ct);
        }

        public async Task<Patient?> GetPatientByUserAsync(int userId, CancellationToken ct)
        {
            return await _context.Patients
                .Include(_ => _.User)
                .FirstOrDefaultAsync(_ => _.UserId == userId, ct);
        }

        public async Task<decimal?> GetPatientBalanceAsync(int userId, CancellationToken ct)
        {
            return await _context.Users
                .Where(_ => _.Id == userId)
                .Select(_ => (decimal?)_.Money)
                .FirstOrDefaultAsync(ct);
        }

        public Task DeletePatientAsync(Patient patient, CancellationToken ct = default)
        {
            _context.Patients.Remove(patient);

            return Task.CompletedTask;
        }
    }
}
