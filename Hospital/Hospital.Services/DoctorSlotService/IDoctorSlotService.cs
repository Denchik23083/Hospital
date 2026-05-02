using Hospital.Core.Models.Response;

namespace Hospital.Services.DoctorSlotService
{
    public interface IDoctorSlotService
    {
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int userId);

        Task<IEnumerable<DoctorSlotBookingResponse>> GetAllDoctorSlotsTimesByDoctorAsync(DateOnly date, int userId);

        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId);
        
        Task<IEnumerable<DoctorSlotResponse>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId);

        Task<IEnumerable<DateOnly>> GetAllAdminDoctorSlotsDatesAsync(int doctorId);

        Task<IEnumerable<DoctorSlotResponse>> GetAllAdminDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date);

        Task AddDoctorSlotsAsync(DateOnly date, int userId);
    }
}