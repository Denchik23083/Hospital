using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;

namespace Hospital.Services.PatientService
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientWithUserResponse>> GetAllPatientsAsync();

        Task<PatientWithUserResponse> GetPatientByUserAsync(int userId);

        Task<decimal> GetPatientBalanceAsync(int userId);

        Task UpdatePatientAsync(PatientRequest model, int userId);
        
        Task ReplenishBalanceAsync(PatientReplenishBalanceRequest model, int userId);
        
        Task DeletePatientAsync(int patientId);
    }
}