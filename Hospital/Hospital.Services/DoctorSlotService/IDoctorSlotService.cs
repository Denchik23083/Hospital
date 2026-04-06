
namespace Hospital.Services.DoctorSlotService
{
    public interface IDoctorSlotService
    {
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId);
    }
}