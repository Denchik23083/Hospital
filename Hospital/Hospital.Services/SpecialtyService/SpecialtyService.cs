using Hospital.Core.Models.Response;
using Hospital.Repositories.SpecialtyRepository;

namespace Hospital.Services.SpecialtyService
{
    public class SpecialtyService(ISpecialtyRepository repository) : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repository = repository;

        public async Task<IEnumerable<SpecialtyResponse>> GetAllSpecialtiesAsync()
        {
            return await _repository.GetAllSpecialtiesAsync();
        }

        public async Task<decimal> GetSpecialtyPriceAsync(int specialtyId)
        {
            return await _repository.GetSpecialtyPriceAsync(specialtyId);
        }
    }
}
