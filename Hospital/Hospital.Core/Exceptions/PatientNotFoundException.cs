namespace Hospital.Core.Exceptions
{
    public class PatientNotFoundException(string message)
        : Exception(message)
    { }
}
