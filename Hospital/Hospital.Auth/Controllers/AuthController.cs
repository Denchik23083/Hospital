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
        public async Task<ActionResult> RegisterAsync(RegisterRequest model)
        {
            await _service.RegisterAsync(model);

            return Created();
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> LoginAsync(LoginRequest model)
        {
            var token = await _service.LoginAsync(model);

            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest model)
        {
            var token = await _service.RefreshTokenAsync(model);

            return Ok(token);
        }
    }
}
