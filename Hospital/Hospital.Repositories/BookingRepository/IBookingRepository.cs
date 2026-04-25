using Hospital.Core.Models.Responce;
using Hospital.Db.Entities;

namespace Hospital.Repositories.BookingRepository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingResponce>> GetAllBookingsAsync(int patientId);

        Task<bool> HasActiveBookingWithDoctorAsync(int patientId, int doctorId);

        Task AddBookingAsync(Booking booking);
    }
}