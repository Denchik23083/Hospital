namespace Hospital.Core.Exceptions
{
    public class SlotAlreadyBookedException(string message)
        : Exception(message)
    { }
}
