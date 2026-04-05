using Hospital.Core.Models.Responce;

namespace Hospital.Services.SpecialtyService
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<SpecialtyResponce>> GetAllSpecialtiesAsync();
    }
}