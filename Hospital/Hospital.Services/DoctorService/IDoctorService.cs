using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;

namespace Hospital.Services.DoctorService
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId);
    
        Task<DoctorWithUserResponse> GetDoctorAsync(int userId);

        Task UpdateDoctorAsync(DoctorRequest model, int userId);
    }
}