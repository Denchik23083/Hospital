namespace Hospital.Core.Models.Response
{
    public class DoctorWithUserResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public string GenderType { get; set; } = string.Empty;

        public SpecialtyResponse? Specialty { get; set; }

        public UserResponse? User { get; set; }
    }
}
