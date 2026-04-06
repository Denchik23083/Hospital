using Hospital.Db.Utilities;

namespace Hospital.Db.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        public int DoctorSlotId { get; set; }

        public DoctorSlot? DoctorSlot { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public BookingStatus BookingStatus { get; set; }
    }
}
