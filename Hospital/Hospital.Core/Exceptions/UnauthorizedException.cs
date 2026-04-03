namespace Hospital.Core.Exceptions
{
    public class UnauthorizedException(string message) 
        : Exception(message) { }
}
