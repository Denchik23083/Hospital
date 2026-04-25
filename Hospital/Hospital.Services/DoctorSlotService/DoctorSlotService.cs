using Hospital.Core.Exceptions;
using Hospital.Core.Models.Responce;
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

        public async Task<IEnumerable<DoctorSlotBookingResponce>> GetAllDoctorSlotsByDoctorAsync(int userId)
        {
            var doctor = await _doctorRepository.GetDoctorAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            return await _repository.GetAllDoctorSlotsByDoctorAsync(doctor.Id);
        }

        public async Task<IEnumerable<DateOnly>> GetAllDoctorSlotsDatesAsync(int doctorId, int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            if (await _bookingRepository.HasActiveBookingWithDoctorAsync(patient.Id, doctorId))
            {
                return [];
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _repository.GetAllDoctorSlotsDatesAsync(doctorId, today);
        }

        public async Task<IEnumerable<DoctorSlotResponce>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date, int userId)
        {
            var patient = await _patientRepository.GetPatientAsync(userId)
                ?? throw new PatientNotFoundException("Patient not found");

            if (await _bookingRepository.HasActiveBookingWithDoctorAsync(patient.Id, doctorId))
            {
                return [];
            }

            return await _repository.GetAllDoctorSlotsTimeByDateAsync(doctorId, date);
        }
    }
}
