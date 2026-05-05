using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.PatientService
{
    public class PatientService(IPatientRepository repository,
            IMapper mapper,
            ILogger<PatientService> logger,
            IUnitOfWorkRepository unitOfWorkRepository) : IPatientService
    {
        private readonly IPatientRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<PatientService> _logger = logger;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<PatientWithUserResponse>> GetAllPatientsAsync()
        {
            return await _repository.GetAllPatientsAsync();
        }

        public async Task<PatientWithUserResponse> GetPatientByUserAsync(int userId)
        {
            var patient = await _repository.GetPatientByUserAsync(userId);

            if (patient is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            return _mapper.Map<PatientWithUserResponse>(patient);
        }

        public async Task<decimal> GetPatientBalanceAsync(int userId)
        {
            return await _repository.GetPatientBalanceAsync(userId);
        }

        public async Task UpdatePatientAsync(PatientRequest model, int userId)
        {
            var patientToUpdate = await _repository.GetPatientByUserAsync(userId);

            if (patientToUpdate is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            patientToUpdate.FirstName = model.FirstName;
            patientToUpdate.LastName = model.LastName;
            patientToUpdate.BirthDate = model.BirthDate;
            patientToUpdate.Phone = model.Phone;
            patientToUpdate.GenderType = model.GenderType;

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task ReplenishBalanceAsync(PatientReplenishBalanceRequest model, int userId)
        {
            var patientToUpdate = await _repository.GetPatientByUserAsync(userId);

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

            patientToUpdate.User.Money = model.Amount;

            await _unitOfWorkRepository.SaveChangesAsync();
        }

        public async Task DeletePatientAsync(int patientId)
        {
            var patientToDelete = await _repository.GetPatientAsync(patientId);

            if (patientToDelete is null)
            {
                _logger.LogWarning("Patient not found");
                throw new PatientNotFoundException("Patient not found");
            }

            await _repository.DeletePatientAsync(patientToDelete);
            await _unitOfWorkRepository.SaveChangesAsync();
        }
    }
}
