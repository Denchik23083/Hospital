using Hospital.Db.Entities;

namespace Hospital.Repositories.SpecialtyRepository
{
    public interface ISpecialtyRepository
    {
        Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync(CancellationToken ct);

        Task<decimal?> GetSpecialtyPriceAsync(int specialtyId, CancellationToken ct);
    }
}