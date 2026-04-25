namespace Hospital.Core.Models.Responce
{
    public class DoctorSlotWithDoctorResponse
    {
        public int Id { get; set; }

        public DoctorResponce? DoctorResponce { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
