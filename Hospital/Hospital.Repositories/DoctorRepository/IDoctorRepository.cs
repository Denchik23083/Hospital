using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorRepository
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<DoctorWithUserResponse>> GetAllDoctorsAsync();

        Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId);

        Task<Doctor?> GetDoctorAsync(int id);

        Task<Doctor?> GetDoctorByUserAsync(int userId);
    }
}