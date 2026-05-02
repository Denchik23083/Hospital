using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Response
{
    public class DoctorWithUserResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public GenderType GenderType { get; set; }

        public TimeSpan WorkDayStart { get; set; }

        public TimeSpan WorkDayEnd { get; set; }

        public SpecialtyResponse? Specialty { get; set; }

        public UserResponse? User { get; set; }
    }
}
