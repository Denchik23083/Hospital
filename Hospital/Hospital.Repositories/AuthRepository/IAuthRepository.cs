using Hospital.Db.Entities;

namespace Hospital.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<bool> IsEmailNotUniqueAsync(string email);

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserAsync(int userId);

        Task RegisterAsync(User user);
    }
}