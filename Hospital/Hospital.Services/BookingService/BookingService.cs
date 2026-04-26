using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;

namespace Hospital.Services.BookingService
{
    public class BookingService(IBookingRepository repository,
            IPatientRepository patientRepository,
            IDoctorSlotRepository doctorSlotRepository,
            IUnitOfWorkRepository unitOfWorkRepository) : IBookingService
    {
        private readonly IBookingRepository _repository = repository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IDoctorSlotRepository _doctorSlotRepository = doctorSlotRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            return await _repository.GetAllPatientBookingsAsync(patient.Id);
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

        public async Task CancelBookingAsync(int id, int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            var booking = await _repository.GetBookingWithPatientAsync(id, patient.Id)
                ?? throw new BookingNotFoundException("Booking not found");

            if (booking.BookingStatus != BookingStatus.Active)
            {
                throw new BookingNotFoundException("Можно менять только активную запись");
            }

            booking.BookingStatus = BookingStatus.Cancelled;

            await _unitOfWorkRepository.SaveChangesAsync();
        }
    }
}
