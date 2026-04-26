using Hospital.Core.Models.Response;

namespace Hospital.Services.AdminService
{
    public interface IAdminService
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        
        Task<UserResponse> GetUserAsync(int userId);

        Task DeleteUserAsync(int userId);
    }
}