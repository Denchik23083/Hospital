namespace Hospital.Core.Exceptions
{
    public class DoctorNotFoundException(string message)
        : Exception(message)
    { }
}
