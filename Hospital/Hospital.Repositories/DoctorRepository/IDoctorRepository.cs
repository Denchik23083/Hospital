using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorRepository
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync(CancellationToken ct);

        Task<IEnumerable<Doctor>> GetAllDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct);

        Task<Doctor?> GetDoctorAsync(int id, CancellationToken ct);

        Task<Doctor?> GetDoctorByUserAsync(int userId, CancellationToken ct);

        Task CreateDoctorAsync(Doctor doctor, CancellationToken ct);

        Task DeleteDoctorAsync(Doctor doctor, CancellationToken ct);
    }
}