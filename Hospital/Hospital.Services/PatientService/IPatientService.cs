using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;

namespace Hospital.Services.PatientService
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientWithUserResponse>> GetAllPatientsAsync(CancellationToken ct);

        Task<PatientWithUserResponse> GetPatientByUserAsync(int userId, CancellationToken ct);

        Task<decimal> GetPatientBalanceAsync(int userId, CancellationToken ct);

        Task UpdatePatientAsync(PatientRequest model, int userId, CancellationToken ct);
        
        Task ReplenishBalanceAsync(PatientReplenishBalanceRequest model, int userId, CancellationToken ct);
        
        Task DeletePatientAsync(int patientId, CancellationToken ct);
    }
}