using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Repositories.PatientRepository;

namespace Hospital.Services.DoctorSlotService
{
    public class DoctorSlotService(IDoctorSlotRepository repository,
            IPatientRepository patientRepository,
            IBookingRepository bookingRepository,
            IDoctorRepository doctorRepository) : IDoctorSlotService
    {
        private readonly IDoctorSlotRepository _repository = repository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IDoctorRepository _doctorRepository = doctorRepository;

        private readonly TimeSpan _startTime = new (09, 00, 00);
        private readonly TimeSpan _endTime = new (16, 00, 00);
        private readonly TimeSpan _breakStart = new (13, 00, 00);
        private readonly TimeSpan _breakEnd = new (14, 00, 00);
        private readonly TimeSpan _slot = new (00, 30, 00);

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesByDoctorAsync(int userId)
        {
            var doctor = await _doctorRepository.GetDoctorAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            return await _repository.GetAllDoctorSlotsDatesByDoctorAsync(doctor.Id);
        }
        
        public async Task<IEnumerable<DoctorSlotBookingResponse>> GetAllDoctorSlotsTimesByDoctorAsync(DateOnly date, int userId)
        {
            var doctor = await _doctorRepository.GetDoctorAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            return await _repository.GetAllDoctorSlotsTimesByDoctorAsync(doctor.Id, date);
        }

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            var doctor = await _doctorRepository.GetDoctorAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            if (await _bookingRepository.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id))
            {
                return [];
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _repository.GetAllDoctorSlotsDatesAsync(doctor.Id, today);
        }

        public async Task<IEnumerable<DoctorSlotResponse>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            var doctor = await _doctorRepository.GetDoctorAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            if (await _bookingRepository.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id))
            {
                return [];
            }

            return await _repository.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date);
        }

        public async Task AddDoctorSlotsAsync(DateOnly date, int userId)
        {
            var doctor = await _doctorRepository.GetDoctorAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            if (await _repository.DoctorSlotsAlreadyExists(doctor.Id, date))
            {
                throw new DoctorSlotAlreadyExistsException($"Doctor slot with {date} already exists");
            }

            var now = _startTime;
            var listDoctorSlots = new List<DoctorSlot>();

            while (now < _endTime)
            {
                if (now >= _breakStart && now < _breakEnd)
                {
                    now = _breakEnd;
                }

                listDoctorSlots.Add(new DoctorSlot
                {
                    DoctorId = doctor.Id,
                    StartTime = now,
                    EndTime = now + _slot,
                    Date = date
                });

                now += _slot;
            }

            await _repository.AddDoctorSlotsAsync(listDoctorSlots);
        }
    }
}
