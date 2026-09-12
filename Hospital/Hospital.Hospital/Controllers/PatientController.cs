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

        [Authorize(Roles = AppRoles.DoctorAdmin)]
        [HttpGet]
        public async Task<ActionResult<PatientWithUserResponse>> GetAllPatientsAsync(CancellationToken ct)
        {
            var patients = await _service.GetAllPatientsAsync(ct);

            return Ok(patients);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet("profile")]
        public async Task<ActionResult<PatientWithUserResponse>> GetPatientByUserAsync(CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var patientWithUser = await _service.GetPatientByUserAsync(userId, ct);

            return Ok(patientWithUser);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetPatientBalanceAsync(CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var balance = await _service.GetPatientBalanceAsync(userId, ct);

            return Ok(balance);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpPut("profile")]
        public async Task<ActionResult> UpdatePatientAsync(PatientRequest model, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.UpdatePatientAsync(model, userId, ct);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpPut("replenish")]
        public async Task<ActionResult> ReplenishBalanceAsync(PatientReplenishBalanceRequest model, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.ReplenishBalanceAsync(model, userId, ct);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{patientId}")]
        public async Task<ActionResult> DeletePatientAsync(int patientId, CancellationToken ct)
        {
            await _service.DeletePatientAsync(patientId, ct);

            return NoContent();
        }
    }
}
