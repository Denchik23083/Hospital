using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.BookingRepository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int patientId);

        Task<IEnumerable<Booking>> GetAllBookingsByDoctorAsync(int doctorId);
        
        Task<IEnumerable<Booking>> GetAllBookingsByPatientAsync(int patientId);

        Task<Booking?> GetBookingWithDoctorAsync(int id, int doctorId);
        
        Task<Booking?> GetBookingWithPatientAsync(int id, int patientId);

        Task<bool> HasActiveBookingWithDoctorAsync(int patientId, int doctorId);

        Task AddBookingAsync(Booking booking);
    }
}