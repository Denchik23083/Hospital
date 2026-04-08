namespace Hospital.Db.Entities
{
    public class DoctorSlot
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public Doctor? Doctor { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public Booking? Booking { get; set; }
    }
}
