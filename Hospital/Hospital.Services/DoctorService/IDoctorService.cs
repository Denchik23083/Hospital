using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;

namespace Hospital.Services.DoctorService
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorWithUserResponse>> GetAllDoctorsAsync();

        Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId);
    
        Task<DoctorWithUserResponse> GetDoctorByUserAsync(int userId);

        Task CreateDoctorAsync(DoctorFullRequest model);

        Task UpdateDoctorByUserAsync(DoctorRequest model, int userId);
        
        Task UpdateDoctorAsync(DoctorFullRequest model, int doctorId);

        Task DeleteDoctorAsync(int doctorId);
    }
}