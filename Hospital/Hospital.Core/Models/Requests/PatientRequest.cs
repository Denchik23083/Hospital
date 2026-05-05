using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Requests
{
    public record class PatientRequest(string FirstName, string LastName,
        DateOnly BirthDate, GenderType GenderType, string Phone);
}
