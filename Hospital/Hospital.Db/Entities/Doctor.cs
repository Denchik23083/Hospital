using Hospital.Db.Utilities;

namespace Hospital.Db.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        public User? User { get; set; }

        public int UserId { get; set; }

        public Specialty? Specialty { get; set; }

        public int SpecialtyId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public GenderType GenderType { get; set; }

        public TimeSpan WorkDayStart { get; set; }

        public TimeSpan WorkDayEnd { get; set; }

        public List<DoctorSlot> DoctorSlots { get; set; } = [];
    }
}
