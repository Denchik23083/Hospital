namespace Hospital.Services.BookingService
{
    public interface IBookingService
    {
        Task CreateBookingAsync(int slotId, int userId);
    }
}