namespace Hospital.Core.Exceptions
{
    public class NotificationNotFoundException(string message)
        : Exception(message)
    { }
}
