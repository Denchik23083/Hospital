using Hospital.Core.Models.Response;

namespace Hospital.Services.GodService
{
    public interface IGodService
    {
        Task<IEnumerable<UserResponse>> GetAllAdminsAsync();
        
        Task<UserResponse> GetAdminAsync(int adminId);

        Task MakeAdminAsync(int userId);

        Task MakeUserAsync(int adminId);
    }
}