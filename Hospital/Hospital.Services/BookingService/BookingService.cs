using Hospital.Core.Exceptions;
using Hospital.Core.Models.Responce;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.BookingService
{
    public class BookingService(HospitalContext context) : IBookingService
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<BookingResponce>> GetAllBookingsAsync(int userId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(_ => _.UserId == userId)
                ?? throw new PatientNotFoundException("Patient not found");

            return await _context.Bookings
                .Where(_ => _.PatientId == patient.Id)
                .Select(_ => new BookingResponce
                {
                    Id = _.Id,
                    BookingStatus = _.BookingStatus,
                    DoctorSlotWithDoctorResponse = new DoctorSlotWithDoctorResponse
                    {
                        Id = _.DoctorSlot!.Id,
                        Date = _.DoctorSlot!.Date,
                        StartTime = _.DoctorSlot!.StartTime,
                        EndTime = _.DoctorSlot!.EndTime,
                        DoctorResponce = new DoctorResponce
                        {
                            Id = _.DoctorSlot!.Doctor!.Id,
                            FirstName = _.DoctorSlot.Doctor.FirstName,
                            LastName = _.DoctorSlot.Doctor.LastName,
                            ExperienceYears = _.DoctorSlot.Doctor.ExperienceYears,
                            GenderType = _.DoctorSlot.Doctor.GenderType.ToString(),
                        }
                    }
                }).ToListAsync();
        }

        public async Task CreateBookingAsync(int slotId, int userId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(_ => _.UserId == userId) 
                ?? throw new PatientNotFoundException("Patient not found");

            var doctorSlot = await _context.DoctorSlots
                .Include(_ => _.Bookings)
                .FirstOrDefaultAsync(_ => _.Id == slotId)
                ?? throw new DoctorSlotNotFoundException("Doctor slot not found");

            if (doctorSlot.Bookings.Any(b => b.BookingStatus == BookingStatus.Active))
            {
                throw new SlotAlreadyBookedException("Slot already booked");
            }

            var patientAlreadyHasActiveBookingWithDoctor = await _context.Bookings
                .AnyAsync(_ => _.PatientId == patient.Id
                    && _.BookingStatus == BookingStatus.Active
                    && _.DoctorSlot != null
                    && _.DoctorSlot.DoctorId == doctorSlot.DoctorId);

            if (patientAlreadyHasActiveBookingWithDoctor)
            {
                throw new SlotAlreadyBookedException("Patient already has an active booking with this doctor");
            }

            var booking = new Booking
            {
                PatientId = patient.Id,
                DoctorSlotId = doctorSlot.Id,
                CreatedAt = DateTime.UtcNow,
                BookingStatus = BookingStatus.Active
            };

            await _context.Bookings.AddAsync(booking);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new SlotAlreadyBookedException("Slot already booked");
            }
        }
    }
}
