using Hospital.Core.Models.Response;

namespace Hospital.Services.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int userId);

        Task CreateBookingAsync(int slotId, int userId);

        Task CancelBookingAsync(int id, int userId);
    }
}