namespace Hospital.Core.Exceptions
{
    public class UserNotFoundException(string message)
        : Exception(message)
    { }
}
