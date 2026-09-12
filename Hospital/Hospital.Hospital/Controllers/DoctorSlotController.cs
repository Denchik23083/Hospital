using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Services.DoctorSlotService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class DoctorSlotController(IDoctorSlotService service) : ControllerBase
    {
        private readonly IDoctorSlotService _service = service;

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpGet("doctor-dates")]
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAllDoctorSlotsDatesByDoctorAsync(CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dates = await _service.GetAllDoctorSlotsDatesByDoctorAsync(userId, ct);

            return Ok(dates);
        }

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpGet("doctor-times")]
        public async Task<ActionResult<IEnumerable<DoctorSlotBookingResponse>>> GetAllDoctorSlotsTimesByDoctorAsync([FromQuery] DateOnly date, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorSlots = await _service.GetAllDoctorSlotsTimesByDoctorAsync(date, userId, ct);

            return Ok(doctorSlots);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet("{doctorId}/available-dates")]
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAllDoctorSlotsDatesAsync(int doctorId, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dates = await _service.GetAllDoctorSlotsDatesAsync(doctorId, userId, ct);

            return Ok(dates);
        }

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet("{doctorId}/available-times")]
        public async Task<ActionResult<IEnumerable<DoctorSlotResponse>>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, [FromQuery]DateOnly date, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorSlots = await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, ct);

            return Ok(doctorSlots);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("{doctorId}/admin/available-dates")]
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAllAdminDoctorSlotsDatesAsync(int doctorId, CancellationToken ct)
        {
            var dates = await _service.GetAllAdminDoctorSlotsDatesAsync(doctorId, ct);

            return Ok(dates);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("{doctorId}/admin/available-times")]
        public async Task<ActionResult<IEnumerable<DoctorSlotResponse>>> GetAllAdminDoctorSlotsTimeByDateAsync(int doctorId, [FromQuery] DateOnly date, CancellationToken ct)
        {
            var doctorSlots = await _service.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date, ct);

            return Ok(doctorSlots);
        }

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpPost]
        public async Task<ActionResult> AddDoctorSlotsAsync([FromQuery] DateOnly date, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.AddDoctorSlotsAsync(date, userId, ct);

            return Created();
        }

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpDelete]
        public async Task<ActionResult> DeleteDoctorSlotsAsync(CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.DeleteDoctorSlotsAsync(userId, ct);

            return NoContent();
        }
    }
}
