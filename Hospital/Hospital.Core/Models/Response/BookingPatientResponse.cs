using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Response
{
    public class BookingPatientResponse
    {
        public int Id { get; set; }

        public PatientResponse? PatientResponse { get; set; }

        public string BookingStatus { get; set; } = string.Empty;
    }
}
