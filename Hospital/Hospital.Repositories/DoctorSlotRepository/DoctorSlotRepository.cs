using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.DoctorSlotRepository
{
    public class DoctorSlotRepository(HospitalContext context) : IDoctorSlotRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int doctorId, CancellationToken ct)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId)
                .Select(_ => _.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<DoctorSlot>> GetAllDoctorSlotsTimesByDoctorAsync(int doctorId, DateOnly date, CancellationToken ct)   
        {
            return await _context.DoctorSlots
                .Include(_ => _.Bookings)
                .ThenInclude(_ => _.Patient)
                .Where(_ => _.DoctorId == doctorId && _.Date == date)
                .OrderBy(_ => _.StartTime)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, DateOnly today, TimeSpan currentTime, CancellationToken ct)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active)
                    && (_.Date > today || (_.Date == today && _.StartTime >= currentTime)))
                .Select(_ => _.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<DoctorSlot>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, DateOnly today, TimeSpan currentTime, CancellationToken ct)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && _.Date == date
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active)
                    && (date > today || (date == today && _.StartTime >= currentTime)))
                .OrderBy(s => s.StartTime)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<int>> GetAllExpiredDoctorSlotsAsync(int doctorId, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && _.Date < today
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active))
                .Select(slot => slot.Id)
                .ToListAsync(ct);
        }

        public async Task<DoctorSlot?> GetDoctorSlotAsync(int slotId, CancellationToken ct)
        {
            return await _context.DoctorSlots
                .Include(_ => _.Doctor)
                .ThenInclude(_ => _!.User)
                .Include(_ => _.Doctor)
                .ThenInclude(_ => _!.Specialty)
                .Include(_ => _.Bookings)
                .FirstOrDefaultAsync(_ => _.Id == slotId, ct);
        }

        public async Task<bool> DoctorSlotsAlreadyExistsAsync(int doctorId, DateOnly date, CancellationToken ct)
        {
            return await _context.DoctorSlots
                .AnyAsync(_ => _.DoctorId == doctorId && _.Date == date, ct);
        }

        public async Task AddDoctorSlotsAsync(List<DoctorSlot> doctorSlots, CancellationToken ct)
        {
            await _context.DoctorSlots.AddRangeAsync(doctorSlots, ct);
        }

        public async Task DeleteDoctorSlotsAsync(List<int> expiredDoctorSlots, CancellationToken ct)
        {
            await _context.Bookings
                .Where(booking => expiredDoctorSlots.Contains(booking.DoctorSlotId))
                .ExecuteDeleteAsync(ct);

            await _context.DoctorSlots
                .Where(slot => expiredDoctorSlots.Contains(slot.Id))
                .ExecuteDeleteAsync(ct);
        }
    }
}
