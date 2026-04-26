using Hospital.Core.Models.Responce;

namespace Hospital.Services.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponce>> GetAllPatientBookingsAsync(int userId);

        Task CreateBookingAsync(int slotId, int userId);
    }
}