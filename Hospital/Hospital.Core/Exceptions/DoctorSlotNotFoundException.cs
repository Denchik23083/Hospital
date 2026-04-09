namespace Hospital.Core.Exceptions
{
    public class DoctorSlotNotFoundException(string message)
        : Exception(message)
    { }
}
