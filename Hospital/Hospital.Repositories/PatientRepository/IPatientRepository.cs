using Hospital.Db.Entities;

namespace Hospital.Repositories.PatientRepository
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByUserAsync(int userId);
    }
}