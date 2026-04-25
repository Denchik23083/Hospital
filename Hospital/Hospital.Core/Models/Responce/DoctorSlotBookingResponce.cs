namespace Hospital.Core.Models.Responce
{
    public class DoctorSlotBookingResponce
    {
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public BookingPatientResponce? LastBooking { get; set; }
    }
}
