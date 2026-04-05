using Hospital.Core.Models.Responce;
using Hospital.Core.Utilities;
using Hospital.Services.DoctorService;
using Hospital.Services.SpecialtyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class SpecialtyController(ISpecialtyService service,
                IDoctorService doctorService) : ControllerBase
    {
        private readonly ISpecialtyService _service = service;
        private readonly IDoctorService _doctorService = doctorService;

        [Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialtyResponce>>> GetAllSpecialtiesAsync()
        {
            var specialties = await _service.GetAllSpecialtiesAsync();

            return Ok(specialties);
        }

        [Authorize(Roles = AppRoles.PatientAdminGod)]
        [HttpGet("{specialtyId}/doctors")]
        public async Task<ActionResult<IEnumerable<DoctorResponce>>> GetAllDoctorsBySpecialtyAsync(int specialtyId)
        {
            var doctorsBySpecialty = await _doctorService.GetAllDoctorsBySpecialtyAsync(specialtyId);

            return Ok(doctorsBySpecialty);
        }
    }
}
