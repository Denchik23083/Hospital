using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.PatientRepository
{
    public class PatientRepository(HospitalContext context) : IPatientRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<Patient?> GetPatientAsync(int userId)
        {
            return await _context.Patients
                    .FirstOrDefaultAsync(_ => _.UserId == userId);
        }
    }
}
