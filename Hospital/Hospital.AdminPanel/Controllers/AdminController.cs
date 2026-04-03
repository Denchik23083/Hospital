using Hospital.Core.Models.Responce;
using Hospital.Core.Utilities;
using Hospital.Services.AdminService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.AdminPanel.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminController(IAdminService service) : ControllerBase
    {
        private readonly IAdminService _service = service;

        [Authorize(Roles = AppRoles.GodAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponce>>> GetAllUsersAsync()
        {
            var users = await _service.GetAllUsersAsync();

            return Ok(users);
        }

        [Authorize(Roles = AppRoles.GodAdmin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponce>> GetUserAsync(int id)
        {
            var user = await _service.GetUserAsync(id);

            return Ok(user);
        }

        [Authorize(Roles = AppRoles.GodAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync(int id)
        {
            await _service.DeleteUserAsync(id);

            return NoContent();
        }
    }
}
