using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.PatientRepository
{
    public interface IPatientRepository
    {
        Task<IEnumerable<PatientWithUserResponse>> GetAllPatientsAsync();

        Task<Patient?> GetPatientAsync(int id);

        Task<Patient?> GetPatientByUserAsync(int userId);

        Task<decimal> GetPatientBalanceAsync(int userId);

        Task DeletePatientAsync(Patient patient);
    }
}