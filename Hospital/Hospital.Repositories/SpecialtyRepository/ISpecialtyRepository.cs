using Hospital.Core.Models.Response;

namespace Hospital.Repositories.SpecialtyRepository
{
    public interface ISpecialtyRepository
    {
        Task<IEnumerable<SpecialtyResponse>> GetAllSpecialtiesAsync();

        Task<decimal> GetSpecialtyPriceAsync(int specialtyId);
    }
}