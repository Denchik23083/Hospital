using Hospital.Core.Models.Response;
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

        [Authorize(Roles = AppRoles.PatientAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialtyResponse>>> GetAllSpecialtiesAsync(CancellationToken ct)
        {
            var specialties = await _service.GetAllSpecialtiesAsync(ct);

            return Ok(specialties);
        }

        [Authorize(Roles = AppRoles.PatientAdmin)]
        [HttpGet("{specialtyId}/price")]
        public async Task<ActionResult<decimal>> GetSpecialtyPriceAsync(int specialtyId, CancellationToken ct)
        {
            var price = await _service.GetSpecialtyPriceAsync(specialtyId, ct);

            return Ok(price);
        }

        [Authorize(Roles = AppRoles.PatientAdmin)]
        [HttpGet("{specialtyId}/doctors")]
        public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetAllDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct)
        {
            var doctorsBySpecialty = await _doctorService.GetAllDoctorsBySpecialtyAsync(specialtyId, ct);

            return Ok(doctorsBySpecialty);
        }
    }
}
