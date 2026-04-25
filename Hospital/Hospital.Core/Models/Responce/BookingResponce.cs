using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Responce
{
    public class BookingResponce
    {
        public int Id { get; set; }

        public DoctorSlotWithDoctorResponse? DoctorSlotWithDoctorResponse { get; set; }

        public BookingStatus BookingStatus { get; set; }
    }
}
