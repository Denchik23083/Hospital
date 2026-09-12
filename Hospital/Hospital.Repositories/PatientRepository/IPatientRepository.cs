using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.PatientRepository
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(CancellationToken ct);

        Task<Patient?> GetPatientAsync(int id, CancellationToken ct);

        Task<Patient?> GetPatientByUserAsync(int userId, CancellationToken ct);

        Task<decimal?> GetPatientBalanceAsync(int userId, CancellationToken ct);

        Task DeletePatientAsync(Patient patient, CancellationToken ct);
    }
}