using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.PatientService
{
    public class PatientService(IPatientRepository repository,
            IMapper mapper,
            ILogger<PatientService> logger,
            IAuthRepository authRepository, 
            IBookingRepository bookingRepository,
            INotificationRepository notificationRepository,
            IUnitOfWorkRepository unitOfWorkRepository) : IPatientService
    {
        private readonly IPatientRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<PatientService> _logger = logger;
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<PatientWithUserResponse>> GetAllPatientsAsync(CancellationToken ct)
        {
            var patients = await _repository.GetAllPatientsAsync(ct);

            return _mapper.Map<IEnumerable<PatientWithUserResponse>>(patients);
        }

        public async Task<PatientWithUserResponse> GetPatientByUserAsync(int userId, CancellationToken ct)
        {
            var patient = await _repository.GetPatientByUserAsync(userId, ct);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            return _mapper.Map<PatientWithUserResponse>(patient);
        }

        public async Task<decimal> GetPatientBalanceAsync(int userId, CancellationToken ct)
        {
            var patientBalance = await _repository.GetPatientBalanceAsync(userId, ct);

            if (patientBalance is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException($"User with Id {userId} was not found.");
            }

            return patientBalance.Value;
        }

        public async Task UpdatePatientAsync(PatientRequest model, int userId, CancellationToken ct)
        {
            var patientToUpdate = await _repository.GetPatientByUserAsync(userId, ct);

            if (patientToUpdate is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            if (patientToUpdate.User is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException("User not found");
            }

            if (patientToUpdate.User.Email != model.Email)
            {
                if (await _authRepository.IsEmailNotUniqueAsync(model.Email, ct))
                {
                    _logger.LogWarning("User with this {Email} email is already exist", model.Email);
                    throw new ConflictException(model.Email);
                }

                patientToUpdate.User.Email = model.Email;
            }

            patientToUpdate.FirstName = model.FirstName;
            patientToUpdate.LastName = model.LastName;
            patientToUpdate.BirthDate = model.BirthDate;
            patientToUpdate.Phone = model.Phone;
            patientToUpdate.GenderType = model.GenderType;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                patientToUpdate.User.PasswordHash = passwordHasher.HashPassword(patientToUpdate.User, model.Password);
            }

            await _unitOfWorkRepository.SaveChangesAsync(ct);
        }

        public async Task ReplenishBalanceAsync(PatientReplenishBalanceRequest model, int userId, CancellationToken ct)
        {
            var patientToUpdate = await _repository.GetPatientByUserAsync(userId, ct);

            if (patientToUpdate is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            if (patientToUpdate.User is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException("User not found");
            }

            patientToUpdate.User.Money += model.Amount;

            await _unitOfWorkRepository.SaveChangesAsync(ct);
        }

        public async Task DeletePatientAsync(int patientId, CancellationToken ct)
        {
            var patientToDelete = await _repository.GetPatientAsync(patientId, ct);

            if (patientToDelete is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            if (patientToDelete.User is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException("User not found");
            }

            await using var transaction = await _unitOfWorkRepository.BeginTransactionAsync(ct);

            try
            {
                var bookings = await _bookingRepository.GetAllBookingsByPatientAsync(patientToDelete.Id, ct);

                foreach (var booking in bookings)
                {
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

                    patientToDelete.User.Money += price;
                    booking.DoctorSlot.Doctor.User.Money -= price;

                    await _notificationRepository.AddNotificationAsync(new Notification
                    {
                        UserId = booking.DoctorSlot.Doctor.User.Id,
                        CreatedAt = DateTime.UtcNow,
                        Message = $"Запись отменена. Пациент {patientToDelete.FirstName} {patientToDelete.LastName} был удалён."
                    }, ct);
                }

                await _repository.DeletePatientAsync(patientToDelete, ct);
                await _unitOfWorkRepository.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);

                _logger.LogError(ex, "Error during patient transaction");

                throw;
            }
        }
    }
}
