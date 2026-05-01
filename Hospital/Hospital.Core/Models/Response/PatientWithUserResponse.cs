using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Response
{
    public class PatientWithUserResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public GenderType GenderType { get; set; }

        public string Phone { get; set; } = string.Empty;

        public UserResponse? User { get; set; }
    }
}
