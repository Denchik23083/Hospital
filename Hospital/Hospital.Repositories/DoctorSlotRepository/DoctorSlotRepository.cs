using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.DoctorSlotRepository
{
    public class DoctorSlotRepository(HospitalContext context) : IDoctorSlotRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int doctorId)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId)
                .Select(_ => _.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSlotBookingResponse>> GetAllDoctorSlotsTimesByDoctorAsync(int doctorId, DateOnly date)   
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && _.Date == date)
                .OrderBy(_ => _.StartTime)
                .Select(_ => new DoctorSlotBookingResponse
                {
                    Id = _.Id,
                    Date = _.Date,
                    StartTime = _.StartTime,
                    EndTime = _.EndTime,
                    LastBooking = _.Bookings
                        .OrderByDescending(b => b.CreatedAt)
                        .Select(_ => new BookingPatientResponse
                        {
                            Id = _.Id,
                            BookingStatus = _.BookingStatus.ToString(),
                            PatientResponse = new PatientResponse
                            {
                                Id = _.Patient!.Id,
                                FirstName = _.Patient.FirstName,
                                LastName = _.Patient.LastName,
                                BirthDate = _.Patient.BirthDate,
                                GenderType = _.Patient.GenderType.ToString(),
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

        public async Task<IEnumerable<DoctorSlotResponse>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date)
        {
            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId
                    && _.Date == date
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active))
                .OrderBy(s => s.StartTime)
                .Select(_ => new DoctorSlotResponse
                {
                    Id = _.Id,
                    Date = _.Date,
                    DoctorId = _.DoctorId,
                    StartTime = _.StartTime,
                    EndTime = _.EndTime
                }).ToListAsync();
        }

        public async Task<DoctorSlot?> GetDoctorSlotAsync(int slotId)
        {
            return await _context.DoctorSlots
                .Include(_ => _.Bookings)
                .FirstOrDefaultAsync(_ => _.Id == slotId);
        }

        public async Task<bool> DoctorSlotsAlreadyExists(int doctorId, DateOnly date)
        {
            return await _context.DoctorSlots
                .AnyAsync(_ => _.DoctorId == doctorId && _.Date == date);
        }

        public async Task AddDoctorSlotsAsync(List<DoctorSlot> doctorSlots)
        {
            await _context.DoctorSlots.AddRangeAsync(doctorSlots);

            await _context.SaveChangesAsync();
        }
    }
}
