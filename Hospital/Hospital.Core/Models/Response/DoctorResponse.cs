namespace Hospital.Core.Models.Response
{
    public class DoctorResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public string GenderType { get; set; } = string.Empty;
    }
}
