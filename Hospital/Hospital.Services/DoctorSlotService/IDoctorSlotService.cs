using Hospital.Core.Models.Response;

namespace Hospital.Services.DoctorSlotService
{
    public interface IDoctorSlotService
    {
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int userId, CancellationToken ct);

        Task<IEnumerable<DoctorSlotBookingResponse>> GetAllDoctorSlotsTimesByDoctorAsync(DateOnly date, int userId, CancellationToken ct);

        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId, CancellationToken ct);
        
        Task<IEnumerable<DoctorSlotResponse>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId, CancellationToken ct);

        Task<IEnumerable<DateOnly>> GetAllAdminDoctorSlotsDatesAsync(int doctorId, CancellationToken ct);

        Task<IEnumerable<DoctorSlotResponse>> GetAllAdminDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, CancellationToken ct);

        Task AddDoctorSlotsAsync(DateOnly date, int userId, CancellationToken ct);

        Task DeleteDoctorSlotsAsync(int userId, CancellationToken ct);
    }
}