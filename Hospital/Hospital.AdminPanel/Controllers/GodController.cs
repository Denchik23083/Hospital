using Hospital.Core.Models.Responce;
using Hospital.Core.Utilities;
using Hospital.Services.GodService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.AdminPanel.Controllers
{
    [Route("api/admin-panel/[controller]")]
    [ApiController]
    public class GodController(IGodService service) : ControllerBase
    {
        private readonly IGodService _service = service;

        [Authorize(Roles = AppRoles.God)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponce>>> GetAllAdminsAsync()
        {
            var admins = await _service.GetAllAdminsAsync();

            return Ok(admins);
        }

        [Authorize(Roles = AppRoles.God)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponce>> GetAdminAsync(int id)
        {
            var admin = await _service.GetAdminAsync(id);

            return Ok(admin);
        }

        [Authorize(Roles = AppRoles.God)]
        [HttpPost("{id}/make-admin")]
        public async Task<ActionResult> MakeAdminAsync(int id)
        {
            await _service.MakeAdminAsync(id);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.God)]
        [HttpPost("{id}/make-user")]
        public async Task<ActionResult> MakeUserAsync(int id)
        {
            await _service.MakeUserAsync(id);

            return NoContent();
        }
    }
}
