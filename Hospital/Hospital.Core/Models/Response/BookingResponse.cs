using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Response
{
    public class BookingResponse
    {
        public int Id { get; set; }

        public DoctorSlotWithDoctorResponse? DoctorSlotWithDoctorResponse { get; set; }

        public BookingStatus BookingStatus { get; set; }
    }
}
