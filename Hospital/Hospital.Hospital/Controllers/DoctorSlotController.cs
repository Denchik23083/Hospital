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
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAllDoctorSlotsDatesByDoctorAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dates = await _service.GetAllDoctorSlotsDatesByDoctorAsync(userId);

            return Ok(dates);
        }

        [Authorize(Roles = AppRoles.Doctor)]
        [HttpGet("doctor-times")]
        public async Task<ActionResult<IEnumerable<DoctorSlotBookingResponse>>> GetAllDoctorSlotsTimesByDoctorAsync([FromQuery] DateOnly date)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorSlots = await _service.GetAllDoctorSlotsTimesByDoctorAsync(date, userId);

            return Ok(doctorSlots);
        }

        [Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet("{doctorId}/available-dates")]
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAllDoctorSlotsDatesAsync(int doctorId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dates = await _service.GetAllDoctorSlotsDatesAsync(doctorId, userId);

            return Ok(dates);
        }

        [Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet("{doctorId}/available-times")]
        public async Task<ActionResult<IEnumerable<DoctorSlotResponse>>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, [FromQuery]DateOnly date)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorSlots = await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId);

            return Ok(doctorSlots);
        }
    }
}
