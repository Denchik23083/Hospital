using Hospital.Core.Models.Response;
using Hospital.Repositories.DoctorRepository;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(IDoctorRepository repository) : IDoctorService
    {
        private readonly IDoctorRepository _repository = repository;

        public async Task<IEnumerable<DoctorResponse>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _repository.GetAllDoctorsBySpecialtyAsync(specialtyId);
        }
    }
}
