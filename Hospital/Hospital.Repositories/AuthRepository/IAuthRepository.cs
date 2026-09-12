using Hospital.Db.Entities;

namespace Hospital.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<bool> IsEmailNotUniqueAsync(string email, CancellationToken ct);

        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct);

        Task<User?> GetUserAsync(int userId, CancellationToken ct);

        Task RegisterAsync(User user, CancellationToken ct);
    }
}