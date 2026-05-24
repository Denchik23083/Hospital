using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorSlotRepository
{
    public interface IDoctorSlotRepository
    {
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int doctorId);
        
        Task<IEnumerable<DoctorSlotBookingResponse>> GetAllDoctorSlotsTimesByDoctorAsync(int doctorId, DateOnly date);

        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, DateOnly today);

        Task<IEnumerable<DoctorSlotResponse>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date);

        Task<IEnumerable<int>> GetAllExpiredDoctorSlotsAsync(int doctorId);

        Task<DoctorSlot?> GetDoctorSlotAsync(int slotId);

        Task<bool> DoctorSlotsAlreadyExistsAsync(int doctorId, DateOnly date);

        Task AddDoctorSlotsAsync(List<DoctorSlot> doctorSlots);
        
        Task DeleteDoctorSlotsAsync(List<int> expiredDoctorSlots);
    }
}