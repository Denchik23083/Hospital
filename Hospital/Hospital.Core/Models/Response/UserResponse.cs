namespace Hospital.Core.Models.Response
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public decimal Money { get; set; }
    }
}
