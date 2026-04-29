using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.PatientRepository
{
    public class PatientRepository(HospitalContext context) : IPatientRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<Patient?> GetPatientByUserAsync(int userId)
        {
            return await _context.Patients
                .Include(_ => _.User)
                .FirstOrDefaultAsync(_ => _.UserId == userId);
        }
    }
}
