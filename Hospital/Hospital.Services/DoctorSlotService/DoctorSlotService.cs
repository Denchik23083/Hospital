using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.DoctorSlotService
{
    public class DoctorSlotService(IDoctorSlotRepository repository,
            IPatientRepository patientRepository,
            IBookingRepository bookingRepository,
            IDoctorRepository doctorRepository,
            ILogger<DoctorSlotService> logger,
            IUnitOfWorkRepository unitOfWorkRepository) : IDoctorSlotService
    {
        private readonly IDoctorSlotRepository _repository = repository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IDoctorRepository _doctorRepository = doctorRepository;
        private readonly ILogger<DoctorSlotService> _logger = logger;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
        
        private readonly TimeSpan _breakStart = new (13, 00, 00);
        private readonly TimeSpan _breakEnd = new (14, 00, 00);
        private readonly TimeSpan _slot = new (00, 30, 00);

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            return await _repository.GetAllDoctorSlotsDatesByDoctorAsync(doctor.Id);
        }
        
        public async Task<IEnumerable<DoctorSlotBookingResponse>> GetAllDoctorSlotsTimesByDoctorAsync(DateOnly date, int userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            return await _repository.GetAllDoctorSlotsTimesByDoctorAsync(doctor.Id, date);
        }

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var doctor = await _doctorRepository.GetDoctorAsync(doctorId);
            
            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            if (await _bookingRepository.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id))
            {
                return [];
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _repository.GetAllDoctorSlotsDatesAsync(doctor.Id, today);
        }

        public async Task<IEnumerable<DoctorSlotResponse>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId)
        {
            var patient = await _patientRepository.GetPatientByUserAsync(userId);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            var doctor = await _doctorRepository.GetDoctorAsync(doctorId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            if (await _bookingRepository.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id))
            {
                return [];
            }

            return await _repository.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date);
        }

        public async Task<IEnumerable<DateOnly>> GetAllAdminDoctorSlotsDatesAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorAsync(doctorId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _repository.GetAllDoctorSlotsDatesAsync(doctor.Id, today);
        }

        public async Task<IEnumerable<DoctorSlotResponse>> GetAllAdminDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date)
        {
            var doctor = await _doctorRepository.GetDoctorAsync(doctorId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            return await _repository.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date);
        }

        public async Task AddDoctorSlotsAsync(DateOnly date, int userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            if (await _repository.DoctorSlotsAlreadyExistsAsync(doctor.Id, date))
            {
                _logger.LogWarning("Doctor slot with this date already exists");
                throw new DoctorSlotAlreadyExistsException($"Doctor slot with {date} already exists");
            }

            var workDayStart = doctor.WorkDayStart;
            var workDayEnd = doctor.WorkDayEnd;

            var listDoctorSlots = new List<DoctorSlot>();

            while (workDayStart + _slot <= workDayEnd)
            {
                if (workDayStart >= _breakStart && workDayStart < _breakEnd)
                {
                    workDayStart = _breakEnd;
                    continue;
                }

                listDoctorSlots.Add(new DoctorSlot
                {
                    DoctorId = doctor.Id,
                    StartTime = workDayStart,
                    EndTime = workDayStart + _slot,
                    Date = date
                });

                workDayStart += _slot;
            }

            await _repository.AddDoctorSlotsAsync(listDoctorSlots);
            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task DeleteDoctorSlotsAsync(int userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            var expiredDoctorSlots = await _repository.GetAllExpiredDoctorSlotsAsync(doctor.Id);

            if (expiredDoctorSlots.Any())
            {
                await _repository.DeleteDoctorSlotsAsync([.. expiredDoctorSlots]);
            }
        }
    }
}
