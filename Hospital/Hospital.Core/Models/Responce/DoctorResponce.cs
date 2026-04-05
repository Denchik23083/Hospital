using Hospital.Db.Entities;
using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Responce
{
    public class DoctorResponce
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public string GenderType { get; set; } = string.Empty;
    }
}
