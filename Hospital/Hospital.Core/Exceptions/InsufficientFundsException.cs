namespace Hospital.Core.Exceptions
{
    public class InsufficientFundsException(string message)
        : Exception(message)
    { }
}

