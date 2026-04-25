using Hospital.Core.Models.Responce;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.DoctorSlotRepository
{
    public class DoctorSlotRepository(HospitalContext context) : IDoctorSlotRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<DoctorSlot?> GetDoctorSlotAsync(int slotId)
        {
            return await _context.DoctorSlots
                .Include(_ => _.Bookings)
                .FirstOrDefaultAsync(_ => _.Id == slotId);
        }

        public async Task<IEnumerable<DoctorSlotBookingResponce>> GetAllDoctorSlotsByDoctorAsync(int doctorId)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId)
                .OrderBy(_ => _.Date)
                .ThenBy(_ => _.StartTime)
                .Select(_ => new DoctorSlotBookingResponce
                {
                    Id = _.Id,
                    Date = _.Date,
                    StartTime = _.StartTime,
                    EndTime = _.EndTime,
                    LastBooking = _.Bookings
                        .OrderByDescending(b => b.CreatedAt)
                        .Select(_ => new BookingPatientResponce
                        {
                            Id = _.Id,
                            BookingStatus = _.BookingStatus,
                            PatientResponce = new PatientResponce
                            {
                                Id = _.Patient!.Id,
                                FirstName = _.Patient.FirstName,
                                LastName = _.Patient.LastName,
                                BirthDate = _.Patient.BirthDate,
                                GenderType = _.Patient.GenderType,
                                Phone = _.Patient.Phone
                            }
                        }).FirstOrDefault()
                }).ToListAsync();
        }

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, DateOnly today)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && _.Date >= today
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active))
                .Select(_ => _.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && _.Date == date
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active))
                .OrderBy(s => s.StartTime)
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
