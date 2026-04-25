using Hospital.Core.Models.Responce;

namespace Hospital.Services.DoctorSlotService
{
    public interface IDoctorSlotService
    {
        Task<IEnumerable<DoctorSlotBookingResponce>> GetAllDoctorSlotsByDoctorAsync(int userId);
        
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId);
        
        Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId);

        //Task AddDoctorSlotsForTodayAsync
    }
}