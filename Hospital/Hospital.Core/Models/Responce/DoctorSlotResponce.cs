namespace Hospital.Core.Models.Responce
{
    public class DoctorSlotResponce
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
