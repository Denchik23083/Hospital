using Hospital.Core.Models.Responce;

namespace Hospital.Services.AdminService
{
    public interface IAdminService
    {
        Task<IEnumerable<UserResponce>> GetAllUsersAsync();
        
        Task<UserResponce> GetUserAsync(int userId);

        Task DeleteUserAsync(int userId);
    }
}