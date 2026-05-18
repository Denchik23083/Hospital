using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(IDoctorRepository repository,
            IMapper mapper,
            ILogger<DoctorService> logger,
            IAuthRepository authRepository,
            IBookingRepository bookingRepository,
            INotificationRepository notificationRepository,
            IUnitOfWorkRepository unitOfWorkRepository) : IDoctorService
    {
        private readonly IDoctorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DoctorService> _logger = logger;
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<DoctorWithUserResponse>> GetAllDoctorsAsync()
        {
            return await _repository.GetAllDoctorsAsync();
        }

        public async Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _repository.GetAllDoctorsBySpecialtyAsync(specialtyId);
        }

        public async Task<DoctorWithUserResponse> GetDoctorByUserAsync(int userId)
        {
            var doctor = await _repository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            return _mapper.Map<DoctorWithUserResponse>(doctor);
        }

        public async Task CreateDoctorAsync(DoctorFullRequest model)
        {
            ValidateWorkDay(model.WorkDayStart, model.WorkDayEnd, _logger);

            if (await _authRepository.IsEmailNotUniqueAsync(model.Email))
            {
                _logger.LogWarning("User with this {Email} email is already exist", model.Email);
                throw new ConflictException(model.Email);
            }

            var user = new User
            {
                Email = model.Email,
                RoleType = RoleType.Doctor
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            var mappedDoctor = _mapper.Map<Doctor>(model);
            mappedDoctor.User = user;

            await _repository.CreateDoctorAsync(mappedDoctor);
            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task UpdateDoctorByUserAsync(DoctorRequest model, int userId)
        {
            var doctorToUpdate = await _repository.GetDoctorByUserAsync(userId);

            if (doctorToUpdate is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            doctorToUpdate.FirstName = model.FirstName;
            doctorToUpdate.LastName = model.LastName;
            doctorToUpdate.GenderType = model.GenderType;

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(DoctorFullRequest model, int doctorId)
        {
            ValidateWorkDay(model.WorkDayStart, model.WorkDayEnd, _logger);

            var doctorToUpdate = await _repository.GetDoctorAsync(doctorId);

            if (doctorToUpdate is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            if (doctorToUpdate.User is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException("User not found");
            }

            if (doctorToUpdate.User.Email != model.Email)
            {
                if (await _authRepository.IsEmailNotUniqueAsync(model.Email))
                {
                    _logger.LogWarning("User with this {Email} email is already exist", model.Email);
                    throw new ConflictException(model.Email);
                }

                doctorToUpdate.User.Email = model.Email;
            }

            doctorToUpdate.FirstName = model.FirstName;
            doctorToUpdate.LastName = model.LastName;
            doctorToUpdate.GenderType = model.GenderType;
            doctorToUpdate.ExperienceYears = model.ExperienceYears;
            doctorToUpdate.WorkDayStart = model.WorkDayStart;
            doctorToUpdate.WorkDayEnd = model.WorkDayEnd;
            doctorToUpdate.SpecialtyId = model.SpecialtyId;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                doctorToUpdate.User.PasswordHash = passwordHasher.HashPassword(doctorToUpdate.User, model.Password);
            }

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(int doctorId)
        {
            var doctorToDelete = await _repository.GetDoctorAsync(doctorId);

            if (doctorToDelete is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            if (doctorToDelete.User is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException("User not found");
            }

            if (doctorToDelete.Specialty is null)
            {
                _logger.LogWarning("Specialty not found");
                throw new SpecialtyNotFoundException("Specialty not found");
            }

            await using var transaction = await _unitOfWorkRepository.BeginTransactionAsync();

            try
            {
                var bookings = await _bookingRepository.GetAllBookingsByDoctorAsync(doctorToDelete.Id);

                var totalRefund = bookings.Sum(_ => doctorToDelete.Specialty.Price);

                if (doctorToDelete.User.Money < totalRefund)
                {
                    _logger.LogWarning("Not enough money. Transaction was rollback");
                    throw new InsufficientFundsException("Not enough money");
                }

                foreach (var booking in bookings)
                {
                    if (booking.Patient is null)
                    {
                        _logger.LogWarning("Patient not found. Transaction was rollback");
                        throw new PatientNotFoundException("Patient not found");
                    }

                    if (booking.Patient.User is null)
                    {
                        _logger.LogWarning("User not found. Transaction was rollback");
                        throw new UserNotFoundException("User not found");
                    }

                    booking.Patient.User.Money += doctorToDelete.Specialty.Price;

                    await _notificationRepository.AddNotificationAsync(new Notification
                    {
                        UserId = booking.Patient.User.Id,
                        CreatedAt = DateTime.UtcNow,
                        Message = $"Ваша запись отменена. Просим прощения, врач {doctorToDelete.FirstName} {doctorToDelete.LastName} был удалён."
                    });
                }

                doctorToDelete.User.Money -= totalRefund;

                await _repository.DeleteDoctorAsync(doctorToDelete);

                await _unitOfWorkRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error during doctor transaction");

                throw;
            }
        }

        private static void ValidateWorkDay(TimeSpan workDayStart, TimeSpan workDayEnd, ILogger<DoctorService> _logger)
        {
            var minTime = new TimeSpan(9, 0, 0);
            var maxTime = new TimeSpan(17, 0, 0);

            if (workDayStart < minTime)
            {
                _logger.LogWarning("Work day start cannot be earlier than 09:00");
                throw new DoctorWorkTimeException("Work day start cannot be earlier than 09:00");
            }

            if (workDayEnd > maxTime)
            {
                _logger.LogWarning("Work day end cannot be later than 17:00");
                throw new DoctorWorkTimeException("Work day end cannot be later than 17:00");
            }

            if (workDayStart >= workDayEnd)
            {
                _logger.LogWarning("Work day start must be earlier than work day end");
                throw new DoctorWorkTimeException("Work day start must be earlier than work day end");
            }

            if (!IsValidSlotTime(workDayStart) || !IsValidSlotTime(workDayEnd))
            {
                _logger.LogWarning("Work day time must end with :00 or :30");
                throw new DoctorWorkTimeException("Work day time must end with :00 or :30");
            }
        }

        private static bool IsValidSlotTime(TimeSpan time)
        {
            return time.Minutes is 0 or 30 && time.Seconds == 0;
        }
    }
}
