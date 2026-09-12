using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;

namespace Hospital.Services.DoctorService
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorWithUserResponse>> GetAllDoctorsAsync(CancellationToken ct);

        Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct);
    
        Task<DoctorWithUserResponse> GetDoctorByUserAsync(int userId, CancellationToken ct);

        Task CreateDoctorAsync(DoctorFullRequest model, CancellationToken ct);

        Task UpdateDoctorByUserAsync(DoctorRequest model, int userId, CancellationToken ct);
        
        Task UpdateDoctorAsync(DoctorFullRequest model, int doctorId, CancellationToken ct);

        Task DeleteDoctorAsync(int doctorId, CancellationToken ct);
    }
}