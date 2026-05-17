namespace Hospital.Core.Exceptions
{
    public class SpecialtyNotFoundException(string message)
        : Exception(message)
    { }
}
