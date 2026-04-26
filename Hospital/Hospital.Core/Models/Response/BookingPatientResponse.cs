using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Response
{
    public class BookingPatientResponse
    {
        public int Id { get; set; }

        public PatientResponse? PatientResponse { get; set; }

        public BookingStatus BookingStatus { get; set; }
    }
}
