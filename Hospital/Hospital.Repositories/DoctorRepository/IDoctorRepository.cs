using Hospital.Core.Models.Responce;
using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorRepository
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<DoctorResponce>> GetAllDoctorsBySpecialtyAsync(int specialtyId);

        Task<Doctor?> GetDoctorAsync(int doctorId);
    }
}