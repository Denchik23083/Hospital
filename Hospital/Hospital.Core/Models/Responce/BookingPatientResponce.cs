using Hospital.Db.Entities;
using Hospital.Db.Utilities;

namespace Hospital.Core.Models.Responce
{
    public class BookingPatientResponce
    {
        public int Id { get; set; }

        public PatientResponce? PatientResponce { get; set; }

        public BookingStatus BookingStatus { get; set; }
    }
}
