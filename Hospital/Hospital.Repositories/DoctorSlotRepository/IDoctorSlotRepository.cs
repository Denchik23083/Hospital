using Hospital.Db.Entities;

namespace Hospital.Repositories.DoctorSlotRepository
{
    public interface IDoctorSlotRepository
    {
        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int doctorId, CancellationToken ct);
        
        Task<IEnumerable<DoctorSlot>> GetAllDoctorSlotsTimesByDoctorAsync(int doctorId, DateOnly date, CancellationToken ct);

        Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, DateOnly today, TimeSpan currentTime, CancellationToken ct);

        Task<IEnumerable<DoctorSlot>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, DateOnly today, TimeSpan currentTime, CancellationToken ct);

        Task<IEnumerable<int>> GetAllExpiredDoctorSlotsAsync(int doctorId, CancellationToken ct);

        Task<DoctorSlot?> GetDoctorSlotAsync(int slotId, CancellationToken ct);

        Task<bool> DoctorSlotsAlreadyExistsAsync(int doctorId, DateOnly date, CancellationToken ct);

        Task AddDoctorSlotsAsync(List<DoctorSlot> doctorSlots, CancellationToken ct);
        
        Task DeleteDoctorSlotsAsync(List<int> expiredDoctorSlots, CancellationToken ct);
    }
}