using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Requests
{
    public class DoctorRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public GenderType GenderType { get; set; }
    }
}
