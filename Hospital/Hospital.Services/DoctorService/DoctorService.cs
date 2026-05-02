using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(IDoctorRepository repository,
            IMapper mapper,
            ILogger<DoctorService> logger,
            IUnitOfWorkRepository unitOfWorkRepository) : IDoctorService
    {
        private readonly IDoctorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DoctorService> _logger = logger;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _repository.GetAllDoctorsBySpecialtyAsync(specialtyId);
        }

        public async Task<DoctorWithUserResponse> GetDoctorAsync(int userId)
        {
            var doctor = await _repository.GetDoctorByUserAsync(userId);

            if (doctor is null)
            {
                _logger.LogWarning("Doctor not found");
                throw new DoctorNotFoundException("Doctor not found");
            }

            return _mapper.Map<DoctorWithUserResponse>(doctor);
        }

        public async Task UpdateDoctorAsync(DoctorRequest model, int userId)
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
    }
}
