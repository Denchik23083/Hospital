using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Services.DoctorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class DoctorController(IDoctorService service) : ControllerBase
    {
        private readonly IDoctorService _service = service;

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorWithUserResponse>>> GetAllDoctorsAsync()
        {
            var doctors = await _service.GetAllDoctorsAsync();

            return Ok(doctors);
        }

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpGet("profile")]
        public async Task<ActionResult<DoctorWithUserResponse>> GetDoctorByUserAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorWithUser = await _service.GetDoctorByUserAsync(userId);

            return Ok(doctorWithUser);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<ActionResult> CreateDoctorAsync(DoctorFullRequest model)
        {
            await _service.CreateDoctorAsync(model);

            return Created();
        }

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateDoctorByUserAsync(DoctorRequest model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.UpdateDoctorByUserAsync(model, userId);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPut("{doctorId}")]
        public async Task<ActionResult> UpdateDoctorAsync(DoctorFullRequest model, int doctorId)
        {
            await _service.UpdateDoctorAsync(model, doctorId);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{doctorId}")]
        public async Task<ActionResult> DeleteDoctorAsync(int doctorId)
        {
            await _service.DeleteDoctorAsync(doctorId);

            return NoContent();
        }
    }
}
