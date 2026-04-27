namespace Hospital.Core.Exceptions
{
    public class DoctorSlotAlreadyExistsException(string message)
        : Exception(message)
    { }
}
