using AutoMapper;
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
            IMapper mapper,
            IUnitOfWorkRepository unitOfWorkRepository) : IBookingService
    {
        private readonly IBookingRepository _repository = repository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IDoctorSlotRepository _doctorSlotRepository = doctorSlotRepository;
        private readonly IDoctorRepository _doctorRepository = doctorRepository;
        private readonly ILogger<BookingService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<BookingResponse>> GetAllPatientBookingsAsync(int userId, CancellationToken ct)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId, ct);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var bookings = await _repository.GetAllPatientBookingsAsync(patient.Id, ct);

            return _mapper.Map<IEnumerable<BookingResponse>>(bookings);
        }

        public async Task CreateBookingAsync(int slotId, int userId, CancellationToken ct)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId, ct);
            
            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var doctorSlot = await _doctorSlotRepository.GetDoctorSlotAsync(slotId, ct);

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

            if (await _repository.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, ct))
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

            await using var transaction = await _unitOfWorkRepository.BeginTransactionAsync(ct);

            try
            {
                if (patient.User is null)
                {
                    _logger.LogWarning("User not found. Transaction was rollback");
                    throw new UserNotFoundException("User not found");
                }

                if (doctorSlot.Doctor is null 
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

                await _repository.AddBookingAsync(booking, ct);
                await _unitOfWorkRepository.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);

                _logger.LogError(ex, "Error during booking transaction");
                
                throw;
            }
        }

        public async Task CompleteBookingAsync(int id, int userId, CancellationToken ct)
        {
            var doctor = await _doctorRepository.GetDoctorByUserAsync(userId, ct);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            var booking = await _repository.GetBookingWithDoctorAsync(id, doctor.Id, ct);

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

            await _unitOfWorkRepository.SaveChangesAsync(ct);
        }

        public async Task CancelBookingAsync(int id, int userId, CancellationToken ct)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId, ct);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var booking = await _repository.GetBookingWithPatientAsync(id, patient.Id, ct);

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

            await using var transaction = await _unitOfWorkRepository.BeginTransactionAsync(ct);

            try
            {
                if (patient.User is null)
                {
                    _logger.LogWarning("User not found. Transaction was rollback");
                    throw new UserNotFoundException("User not found");
                }

                if (booking.DoctorSlot is null
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
                await _unitOfWorkRepository.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);

                _logger.LogError(ex, "Error during booking transaction");

                throw;
            }
        }
    }
}
