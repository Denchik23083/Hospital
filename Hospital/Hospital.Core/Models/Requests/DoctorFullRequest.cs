using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Requests
{
    public class DoctorFullRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public GenderType GenderType { get; set; }

        public int ExperienceYears { get; set; }

        public TimeSpan WorkDayStart { get; set; }

        public TimeSpan WorkDayEnd { get; set; }

        public int SpecialtyId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
