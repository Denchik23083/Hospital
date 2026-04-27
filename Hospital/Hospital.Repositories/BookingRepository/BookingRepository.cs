using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.BookingRepository
{
    public class BookingRepository(HospitalContext context) : IBookingRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int patientId)
        {
            return await _context.Bookings
                .Where(_ => _.PatientId == patientId)
                .Select(_ => new BookingResponse
                {
                    Id = _.Id,
                    BookingStatus = _.BookingStatus.ToString(),
                    DoctorSlotWithDoctorResponse = new DoctorSlotWithDoctorResponse
                    {
                        Id = _.DoctorSlot!.Id,
                        Date = _.DoctorSlot!.Date,
                        StartTime = _.DoctorSlot!.StartTime,
                        EndTime = _.DoctorSlot!.EndTime,
                        DoctorResponse = new DoctorResponse
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

        public async Task<Booking?> GetBookingWithDoctorAsync(int id, int doctorId)
        {
            return await _context.Bookings
                .Include(_ => _.DoctorSlot)
                .FirstOrDefaultAsync(_ => _.Id == id
                    && _.DoctorSlot!.DoctorId == doctorId);
        }

        public async Task<Booking?> GetBookingWithPatientAsync(int id, int patientId)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(_ => _.Id == id
                    && _.PatientId == patientId);
        }

        public async Task<bool> HasActiveBookingWithDoctorAsync(int patientId, int doctorId)
        {
            return await _context.Bookings
                .AnyAsync(_ => _.PatientId == patientId
                    && _.BookingStatus == BookingStatus.Active
                    && _.DoctorSlot != null
                    && _.DoctorSlot.DoctorId == doctorId);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);

            await _context.SaveChangesAsync();
        }
    }
}
