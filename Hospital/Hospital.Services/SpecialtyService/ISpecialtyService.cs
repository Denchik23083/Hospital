using Hospital.Core.Models.Response;

namespace Hospital.Services.SpecialtyService
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<SpecialtyResponse>> GetAllSpecialtiesAsync();
    }
}