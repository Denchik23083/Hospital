using Hospital.Core.Models.Responce;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.DoctorSlotService
{
    public class DoctorSlotService(HospitalContext context) : IDoctorSlotService
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId && _.Date >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Select(_ => _.Date)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId &&
                        _.Date == date &&
                        _.Booking == null)
                .Select(_ => new DoctorSlotResponce
                {
                    Id = _.Id,
                    Date = _.Date,
                    DoctorId = _.DoctorId,
                    StartTime = _.StartTime,
                    EndTime = _.EndTime
                }).ToListAsync();
        }
    }
}
