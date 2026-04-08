using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Requests
{
    public record class RegisterRequest(
        string Email, string Password,
        string FirstName, string LastName, string Phone,
        DateOnly BirthDate, GenderType GenderType);
}
