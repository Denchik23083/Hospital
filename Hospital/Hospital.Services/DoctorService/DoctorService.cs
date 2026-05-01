using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Repositories.DoctorRepository;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(IDoctorRepository repository,
            IMapper mapper) : IDoctorService
    {
        private readonly IDoctorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _repository.GetAllDoctorsBySpecialtyAsync(specialtyId);
        }

        public async Task<DoctorWithUserResponse> GetDoctorAsync(int userId)
        {
            var doctor = await _repository.GetDoctorByUserAsync(userId)
                ?? throw new DoctorNotFoundException("Doctor not found");

            return _mapper.Map<DoctorWithUserResponse>(doctor);
        }
    }
}
