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
        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetPatientBalanceAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var balance = await _service.GetPatientBalanceAsync(userId);

            return Ok(balance);
        }
    }
}
