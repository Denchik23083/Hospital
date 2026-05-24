using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.BookingService
{
    public class BookingService(IBookingRepository repository,
            IPatientRepository patientRepository,
            IDoctorSlotRepository doctorSlotRepository,
            IDoctorRepository doctorRepository,
            ILogger<BookingService> logger,
            IUnitOfWorkRepository unitOfWorkRepository) : IBookingService
    {
        private readonly IBookingRepository _repository = repository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IDoctorSlotRepository _doctorSlotRepository = doctorSlotRepository;
        private readonly IDoctorRepository _doctorRepository = doctorRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
        private readonly ILogger<BookingService> _logger = logger;

        public async Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int userId)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }
            
            return await _repository.GetAllPatientBookingsAsync(patient.Id);
        }

        public async Task CreateBookingAsync(int slotId, int userId)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId);
            
            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var doctorSlot = await _doctorSlotRepository.GetDoctorSlotAsync(slotId);

            if (doctorSlot is null)
            {
                _logger.LogWarning("Doctor slot not found");
                throw new DoctorSlotNotFoundException("Doctor slot not found");
            }

            if (doctorSlot.Bookings.Any(b => b.BookingStatus == BookingStatus.Active))
            {
                _logger.LogWarning("Slot already booked");
                throw new SlotAlreadyBookedException("Slot already booked");
            }

            var patientAlreadyHasActiveBookingWithDoctor = await _repository.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId);

            if (patientAlreadyHasActiveBookingWithDoctor)
            {
                _logger.LogWarning("Patient already has an active booking with this doctor");
                throw new SlotAlreadyBookedException("Patient already has an active booking with this doctor");
            }

            var booking = new Booking
            {
                PatientId = patient.Id,
                DoctorSlotId = doctorSlot.Id,
                CreatedAt = DateTime.UtcNow,
                BookingStatus = BookingStatus.Active
            };

            await using var transaction = await _unitOfWorkRepository.BeginTransactionAsync();

            try
            {
                if (patient.User is null 
                    || doctorSlot.Doctor is null 
                    || doctorSlot.Doctor.Specialty is null
                    || doctorSlot.Doctor.User is null)
                {
                    _logger.LogWarning("Doctor not found. Transaction was rollback");
                    throw new DoctorNotFoundException("Doctor not found");
                }

                var price = doctorSlot.Doctor.Specialty.Price;

                if (patient.User.Money < price)
                {
                    _logger.LogWarning("Not enough money. Transaction was rollback");
                    throw new InsufficientFundsException("Not enough money");
                }

                patient.User.Money -= price;
                doctorSlot.Doctor.User.Money += price;

                await _repository.AddBookingAsync(booking);
                await _unitOfWorkRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error during booking transaction");
                
                throw;
            }
        }

        public async Task CompleteBookingAsync(int id, int userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            var booking = await _repository.GetBookingWithDoctorAsync(id, doctor.Id);

            if (booking is null)
            {
                _logger.LogWarning("Booking not found");
                throw new BookingNotFoundException("Booking not found");
            }

            if (booking.BookingStatus != BookingStatus.Active)
            {
                _logger.LogWarning("Can change only active booking");
                throw new BookingNotFoundException("Can change only active booking");
            }

            booking.BookingStatus = BookingStatus.Completed;

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task CancelBookingAsync(int id, int userId)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var booking = await _repository.GetBookingWithPatientAsync(id, patient.Id);

            if (booking is null)
            {
                _logger.LogWarning("Booking not found");
                throw new BookingNotFoundException("Booking not found");
            }

            if (booking.BookingStatus != BookingStatus.Active)
            {
                _logger.LogWarning("Can change only active booking");
                throw new BookingNotFoundException("Can change only active booking");
            }

            await using var transaction = await _unitOfWorkRepository.BeginTransactionAsync();

            try
            {
                if (patient.User is null
                    || booking.DoctorSlot is null
                    || booking.DoctorSlot.Doctor is null
                    || booking.DoctorSlot.Doctor.Specialty is null
                    || booking.DoctorSlot.Doctor.User is null)
                {
                    _logger.LogWarning("Doctor not found. Transaction was rollback");
                    throw new DoctorNotFoundException("Doctor not found");
                }

                var price = booking.DoctorSlot.Doctor.Specialty.Price;

                if (booking.DoctorSlot.Doctor.User.Money < price)
                {
                    _logger.LogWarning("Not enough money. Transaction was rollback");
                    throw new InsufficientFundsException("Not enough money");
                }

                booking.DoctorSlot.Doctor.User.Money -= price;
                patient.User.Money += price;

                booking.BookingStatus = BookingStatus.Cancelled;
                await _unitOfWorkRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error during booking transaction");

                throw;
            }
        }
    }
}
