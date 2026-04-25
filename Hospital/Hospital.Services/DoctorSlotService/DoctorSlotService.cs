using Hospital.Core.Exceptions;
using Hospital.Core.Models.Responce;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.DoctorSlotService
{
    public class DoctorSlotService(HospitalContext context) : IDoctorSlotService
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<DoctorSlotBookingResponce>> GetAllDoctorSlotsByDoctorAsync(int userId)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(_ => _.UserId == userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctor.Id)
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

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId)
        {
            if (await HasActiveBookingWithDoctorAsync(doctorId, userId))
            {
                return [];
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.DoctorSlots
                .Where(_ => _.DoctorId == doctorId 
                    && _.Date >= today
                    && !_.Bookings.Any(_ => _.BookingStatus == BookingStatus.Active)) 
                .Select(_ => _.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId)
        {
            if (await HasActiveBookingWithDoctorAsync(doctorId, userId))
            {
                return [];
            }

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

        private async Task<bool> HasActiveBookingWithDoctorAsync(int doctorId, int userId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(_ => _.UserId == userId)
                ?? throw new PatientNotFoundException("Patient not found");

            return await _context.Bookings
                .AnyAsync(_ => _.PatientId == patient.Id
                    && _.BookingStatus == BookingStatus.Active
                    && _.DoctorSlot != null
                    && _.DoctorSlot.DoctorId == doctorId);
        }
    }
}
