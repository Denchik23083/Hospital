using Hospital.Core.Models.Responce;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorSlotBookingResponce>>> GetAllDoctorSlotsByDoctorAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorSlots = await _service.GetAllDoctorSlotsByDoctorAsync(userId);

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
        public async Task<ActionResult<IEnumerable<DoctorSlotResponce>>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, [FromQuery]DateOnly date)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var doctorSlots = await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId);

            return Ok(doctorSlots);
        }
    }
}
