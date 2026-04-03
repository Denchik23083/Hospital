using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Responce;

namespace Hospital.Services.AuthService
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest model);

        Task<TokenResponse> LoginAsync(LoginRequest model);

        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest model);
    }
}