using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Db.Entities;
using Hospital.Services.DoctorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class DoctorContoller(IDoctorService service) : ControllerBase
    {
        private readonly IDoctorService _service = service;

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpGet("profile")]
        public async Task<ActionResult<DoctorWithUserResponse>> GetDoctorAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorWithUser = await _service.GetDoctorAsync(userId);

            return Ok(doctorWithUser);
        }
    }
}
