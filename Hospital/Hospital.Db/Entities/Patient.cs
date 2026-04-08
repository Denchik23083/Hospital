using Hospital.Db.Utilities;

namespace Hospital.Db.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        public User? User { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public GenderType GenderType { get; set; }

        public string Phone { get; set; } = string.Empty;

        public List<Booking> Bookings { get; set; } = [];
    }
}
