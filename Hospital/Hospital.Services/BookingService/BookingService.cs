using Hospital.Core.Exceptions;
using Hospital.Core.Models.Responce;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Repositories.PatientRepository;

namespace Hospital.Services.BookingService
{
    public class BookingService(IBookingRepository repository,
            IPatientRepository patientRepository,
            IDoctorSlotRepository doctorSlotRepository) : IBookingService
    {
        private readonly IBookingRepository _repository = repository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IDoctorSlotRepository _doctorSlotRepository = doctorSlotRepository;

        public async Task<IEnumerable<BookingResponce>> GetAllBookingsAsync(int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            return await _repository.GetAllBookingsAsync(patient.Id);
        }

        public async Task CreateBookingAsync(int slotId, int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            var doctorSlot = await _doctorSlotRepository.GetDoctorSlotAsync(slotId)
                ?? throw new DoctorSlotNotFoundException("Doctor slot not found");

            if (doctorSlot.Bookings.Any(b => b.BookingStatus == BookingStatus.Active))
            {
                throw new SlotAlreadyBookedException("Slot already booked");
            }

            var patientAlreadyHasActiveBookingWithDoctor = await _repository.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId);

            if (patientAlreadyHasActiveBookingWithDoctor)
            {
                throw new SlotAlreadyBookedException("Patient already has an active booking with this doctor");
            }

            var booking = new Booking
            {
                PatientId = patient.Id,
                DoctorSlotId = doctorSlot.Id,
                CreatedAt = DateTime.UtcNow,
                BookingStatus = BookingStatus.Active
            };

            try
            {
                await _repository.AddBookingAsync(booking);
            }
            catch
            {
                throw new SlotAlreadyBookedException("Slot already booked");
            }
        }
    }
}
