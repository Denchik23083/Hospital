using Hospital.Core.Models.Responce;
using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorSlotRepository
{
    public interface IDoctorSlotRepository
    {
        Task<IEnumerable<DoctorSlotBookingResponce>> GetAllDoctorSlotsByDoctorAsync(int doctorId);

        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, DateOnly today);

        Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date);

        Task<DoctorSlot?> GetDoctorSlotAsync(int slotId);
    }
}