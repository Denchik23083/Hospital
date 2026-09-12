using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.BookingRepository
{
    public class BookingRepository(HospitalContext context) : IBookingRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<Booking>> GetAllPatientBookingsAsync(int patientId, CancellationToken ct)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(_ => _.PatientId == patientId)
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsByDoctorAsync(int doctorId, CancellationToken ct)
        {
            return await _context.Bookings
                .Include(_ => _.Patient)
                .ThenInclude(_ => _!.User)
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ThenInclude(_ => _!.User)
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ThenInclude(_ => _!.Specialty)
                .Where(_ => _.DoctorSlot!.DoctorId == doctorId
                    && _.BookingStatus == BookingStatus.Active)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsByPatientAsync(int patientId, CancellationToken ct)
        {
            return await _context.Bookings
                .Include(_ => _.Patient)
                .ThenInclude(_ => _!.User)
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ThenInclude(_ => _!.User)
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ThenInclude(_ => _!.Specialty)
                .Where(_ => _.PatientId == patientId
                    && _.BookingStatus == BookingStatus.Active)
                .ToListAsync(ct);
        }

        public async Task<Booking?> GetBookingWithDoctorAsync(int id, int doctorId, CancellationToken ct)
        {
            return await _context.Bookings
                .Include(_ => _.DoctorSlot)
                .FirstOrDefaultAsync(_ => _.Id == id
                    && _.DoctorSlot!.DoctorId == doctorId, ct);
        }

        public async Task<Booking?> GetBookingWithPatientAsync(int id, int patientId, CancellationToken ct)
        {
            return await _context.Bookings
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ThenInclude(_ => _!.User)
                .Include(_ => _.DoctorSlot)
                .ThenInclude(_ => _!.Doctor)
                .ThenInclude(_ => _!.Specialty)
                .FirstOrDefaultAsync(_ => _.Id == id
                    && _.PatientId == patientId, ct);
        }

        public async Task<bool> HasActiveBookingWithDoctorAsync(int patientId, int doctorId, CancellationToken ct)
        {
            return await _context.Bookings
                .AnyAsync(_ => _.PatientId == patientId
                    && _.BookingStatus == BookingStatus.Active
                    && _.DoctorSlot != null
                    && _.DoctorSlot.DoctorId == doctorId, ct);
        }

        public async Task AddBookingAsync(Booking booking, CancellationToken ct)
        {
            await _context.Bookings.AddAsync(booking, ct);
        }
    }
}
