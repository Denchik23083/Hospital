using Hospital.Core.Models.Responce;

namespace Hospital.Services.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponce>> GetAllBookingsAsync(int userId);

        Task CreateBookingAsync(int slotId, int userId);
    }
}