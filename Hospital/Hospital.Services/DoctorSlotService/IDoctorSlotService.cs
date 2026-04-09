using Hospital.Core.Models.Responce;

namespace Hospital.Services.DoctorSlotService
{
    public interface IDoctorSlotService
    {
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId);
        
        Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date);
    }
}