using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(IDoctorRepository repository,
            IMapper mapper,
            ILogger<DoctorService> logger,
            IAuthRepository authRepository,
            IUnitOfWorkRepository unitOfWorkRepository) : IDoctorService
    {
        private readonly IDoctorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DoctorService> _logger = logger;
        private readonly IAuthRepository _authRepository = authRepository;
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
            ValidateWorkDay(model.WorkDayStart, model.WorkDayEnd);

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
            var doctor = await _repository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.GenderType = model.GenderType;

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(DoctorFullRequest model, int doctorId)
        {
            ValidateWorkDay(model.WorkDayStart, model.WorkDayEnd);

            var doctor = await _repository.GetDoctorAsync(doctorId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.GenderType = model.GenderType;
            doctor.ExperienceYears = model.ExperienceYears;
            doctor.WorkDayStart = model.WorkDayStart;
            doctor.WorkDayEnd = model.WorkDayEnd;
            doctor.SpecialtyId = model.SpecialtyId;

            if (doctor.User is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException("User not found");
            }

            doctor.User.Email = model.Email;

            var passwordHasher = new PasswordHasher<User>();
            doctor.User.PasswordHash = passwordHasher.HashPassword(doctor.User, model.Password);

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(int doctorId)
        {
            var doctor = await _repository.GetDoctorAsync(doctorId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            await _repository.DeleteDoctorAsync(doctor);
            await _unitOfWorkRepository.SaveChangesAsync();
        }

        private static void ValidateWorkDay(TimeSpan workDayStart, TimeSpan workDayEnd)
        {
            var minTime = new TimeSpan(9, 0, 0);
            var maxTime = new TimeSpan(17, 0, 0);

            if (workDayStart < minTime)
            {
                throw new DoctorWorkTimeException("Work day start cannot be earlier than 09:00");
            }

            if (workDayEnd > maxTime)
            {
                throw new DoctorWorkTimeException("Work day end cannot be later than 17:00");
            }

            if (workDayStart >= workDayEnd)
            {
                throw new DoctorWorkTimeException("Work day start must be earlier than work day end");
            }

            if (!IsValidSlotTime(workDayStart) || !IsValidSlotTime(workDayEnd))
            {
                throw new DoctorWorkTimeException("Work day time must end with :00 or :30");
            }
        }

        private static bool IsValidSlotTime(TimeSpan time)
        {
            return time.Minutes is 0 or 30 && time.Seconds == 0;
        }
    }
}
