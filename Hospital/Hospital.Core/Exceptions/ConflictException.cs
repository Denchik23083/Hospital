namespace Hospital.Core.Exceptions
{
    public class ConflictException(string email) 
        : Exception($"User with email {email} is already exist") { }
}
