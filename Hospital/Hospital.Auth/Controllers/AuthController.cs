using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Auth.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {
        private readonly IAuthService _service = service;

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegisterRequest model, CancellationToken ct)
        {
            await _service.RegisterAsync(model, ct);

            return Created();
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> LoginAsync(LoginRequest model, CancellationToken ct)
        {
            var token = await _service.LoginAsync(model, ct);

            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest model, CancellationToken ct)
        {
            var token = await _service.RefreshTokenAsync(model, ct);

            return Ok(token);
        }
    }
}
