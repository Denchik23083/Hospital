using Hospital.Core.Models.Responce;

namespace Hospital.Services.DoctorService
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorResponce>> GetAllDoctorsBySpecialtyAsync(int specialtyId);
    }
}