using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
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
    }
}
