using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Repositories.SpecialtyRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.SpecialtyService
{
    public class SpecialtyService(ISpecialtyRepository repository, 
            IMapper mapper,
            ILogger<SpecialtyService> logger) : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<SpecialtyService> _logger = logger;

        public async Task<IEnumerable<SpecialtyResponse>> GetAllSpecialtiesAsync(CancellationToken ct)
        {
            var specialties = await _repository.GetAllSpecialtiesAsync(ct);

            return _mapper.Map<IEnumerable<SpecialtyResponse>>(specialties);
        }

        public async Task<decimal> GetSpecialtyPriceAsync(int specialtyId, CancellationToken ct)
        {
            var price = await _repository.GetSpecialtyPriceAsync(specialtyId, ct);

            if (price is null)
            {
                _logger.LogWarning("Specialty not found");
                throw new SpecialtyNotFoundException($"Specialty with Id {specialtyId} was not found.");
            }

            return price.Value;
        }
    }
}
