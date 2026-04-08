using Hospital.Core.Models.Responce;
using Hospital.Core.Utilities;
using Hospital.Services.DoctorService;
using Hospital.Services.DoctorSlotService;
using Hospital.Services.SpecialtyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class DoctorSlotController(IDoctorSlotService service) : ControllerBase
    {
        private readonly IDoctorSlotService _service = service;

        [Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet("{doctorId}/available-dates")]
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAllDoctorSlotsDatesAsync(int doctorId)
        {
            var dates = await _service.GetAllDoctorSlotsDatesAsync(doctorId);

            return Ok(dates);
        }

        /*[Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet("{doctorId}/available-times")]
        public async Task<ActionResult<IEnumerable<SpecialtyResponce>>> GetAllDoctorSlotsTimeByDateAsync(int doctorId, DateOnly date)
        {
            var doctorSpots = await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date);

            return Ok(doctorSpots);
        }

        [Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet("{specialtyId}/doctors")]
        public async Task<ActionResult<IEnumerable<DoctorResponce>>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            var doctorsBySpecialty = await _doctorService.GetAllDoctorsBySpecialtyAsync(specialtyId);

            return Ok(doctorsBySpecialty);
        }*/
        
    }
}
