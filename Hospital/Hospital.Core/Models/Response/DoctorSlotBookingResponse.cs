namespace Hospital.Core.Models.Response
{
    public class DoctorSlotBookingResponse
    {
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public BookingPatientResponse? LastBooking { get; set; }
    }
}
