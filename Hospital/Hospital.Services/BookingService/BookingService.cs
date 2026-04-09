using Hospital.Core.Exceptions;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.BookingService
{
    public class BookingService(HospitalContext context) : IBookingService
    {
        private readonly HospitalContext _context = context;

        public async Task CreateBookingAsync(int slotId, int userId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(_ => _.UserId == userId) 
                ?? throw new PatientNotFoundException("Patient not found");

            var doctorSlot = await _context.DoctorSlots
                .Include(_ => _.Booking)
                .FirstOrDefaultAsync(_ => _.Id == slotId)
                ?? throw new PatientNotFoundException("Doctor slot not found");

            if (doctorSlot.Booking is not null)
            {
                throw new SlotAlreadyBookedException("Slot already booked");
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
