using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Services.PatientService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class PatientController(IPatientService service) : ControllerBase
    {
        private readonly IPatientService _service = service;

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet("profile")]
        public async Task<ActionResult<PatientWithUserResponse>> GetPatientAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var patientWithUser = await _service.GetPatientAsync(userId);

            return Ok(patientWithUser);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetPatientBalanceAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var balance = await _service.GetPatientBalanceAsync(userId);

            return Ok(balance);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpPut]
        public async Task<ActionResult<decimal>> UpdatePatientAsync(PatientRequest model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.UpdatePatientAsync(model, userId);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpPut("replenish")]
        public async Task<ActionResult<decimal>> ReplenishBalanceAsync(PatientReplenishBalanceRequest model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.ReplenishBalanceAsync(model, userId);

            return NoContent();
        }
        
    }
}
