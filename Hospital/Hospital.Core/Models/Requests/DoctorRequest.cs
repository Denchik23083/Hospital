using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Requests
{
    public record class DoctorRequest(string FirstName, string LastName, GenderType GenderType);
}
