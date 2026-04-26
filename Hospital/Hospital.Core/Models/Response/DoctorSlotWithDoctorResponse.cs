namespace Hospital.Core.Models.Response
{
    public class DoctorSlotWithDoctorResponse
    {
        public int Id { get; set; }

        public DoctorResponse? DoctorResponse { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
