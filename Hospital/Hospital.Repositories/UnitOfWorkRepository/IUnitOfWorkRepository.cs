namespace Hospital.Repositories.UnitOfWorkRepository
{
    public interface IUnitOfWorkRepository
    {
        Task SaveChangesAsync();
    }
}