using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;

namespace Hospital.Services.AuthService
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest model, CancellationToken ct);

        Task<TokenResponse> LoginAsync(LoginRequest model, CancellationToken ct);

        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest model, CancellationToken ct);
    }
}