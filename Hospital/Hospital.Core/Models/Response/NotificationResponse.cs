namespace Hospital.Core.Models.Response
{
    public class NotificationResponse
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
