using Hospital.Core.Models.Response;

namespace Hospital.Services.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int userId, CancellationToken ct);

        Task CreateBookingAsync(int slotId, int userId, CancellationToken ct);

        Task CompleteBookingAsync(int id, int userId, CancellationToken ct);

        Task CancelBookingAsync(int id, int userId, CancellationToken ct);
    }
}