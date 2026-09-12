using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.BookingRepository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllPatientBookingsAsync(int patientId, CancellationToken ct);

        Task<IEnumerable<Booking>> GetAllBookingsByDoctorAsync(int doctorId, CancellationToken ct);
        
        Task<IEnumerable<Booking>> GetAllBookingsByPatientAsync(int patientId, CancellationToken ct);

        Task<Booking?> GetBookingWithDoctorAsync(int id, int doctorId, CancellationToken ct);
        
        Task<Booking?> GetBookingWithPatientAsync(int id, int patientId, CancellationToken ct);

        Task<bool> HasActiveBookingWithDoctorAsync(int patientId, int doctorId, CancellationToken ct);

        Task AddBookingAsync(Booking booking, CancellationToken ct);
    }
}