using Hospital.Db.Utilities;

namespace Hospital.Db.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public RoleType RoleType { get; set; }

        public decimal Money { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public Doctor? Doctor { get; set; }

        public Patient? Patient { get; set; }

        public List<Notification> Notifications { get; set; } = [];
    }
}
