using Hospital.Core.Models.Responce;
using Hospital.Repositories.DoctorRepository;

namespace Hospital.Services.DoctorService
{
    public class DoctorService(IDoctorRepository repository) : IDoctorService
    {
        private readonly IDoctorRepository _repository = repository;

        public async Task<IEnumerable<DoctorResponce>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _repository.GetAllDoctorsBySpecialtyAsync(specialtyId);
        }
    }
}
