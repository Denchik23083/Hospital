using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Services.AdminService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.AdminPanel.Controllers
{
    [Route("api/admin-panel/[controller]")]
    [ApiController]
    public class AdminController(IAdminService service) : ControllerBase
    {
        private readonly IAdminService _service = service;

        [Authorize(Roles = AppRoles.AdminGod)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsersAsync()
        {
            var users = await _service.GetAllUsersAsync();

            return Ok(users);
        }

        [Authorize(Roles = AppRoles.AdminGod)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserAsync(int id)
        {
            var user = await _service.GetUserAsync(id);

            return Ok(user);
        }

        [Authorize(Roles = AppRoles.AdminGod)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync(int id)
        {
            await _service.DeleteUserAsync(id);

            return NoContent();
        }
    }
}
