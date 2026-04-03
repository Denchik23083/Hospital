using Hospital.Core.Models.Responce;

namespace Hospital.Services.GodService
{
    public interface IGodService
    {
        Task<IEnumerable<UserResponce>> GetAllAdminsAsync();
        
        Task<UserResponce> GetAdminAsync(int adminId);

        Task MakeAdminAsync(int userId);

        Task MakeUserAsync(int adminId);
    }
}