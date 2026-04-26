using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorRepository
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId);

        Task<Doctor?> GetDoctorAsync(int doctorId);
    }
}