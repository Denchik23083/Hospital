namespace Hospital.Core.Exceptions
{
    public class BookingNotFoundException(string message)
        : Exception(message)
    { }
}
