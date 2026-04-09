using Hospital.Core.Models.Responce;
using Hospital.Core.Utilities;
using Hospital.Services.BookingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class BookingController(IBookingService service) : ControllerBase
    {
        private readonly IBookingService _service = service;

        [HttpPost]
        [Authorize(Roles = AppRoles.PatientAdminGod)]
        public async Task<ActionResult> CreateBookingAsync(BookingResponce model)
        {
            await _service.CreateBookingAsync(model.SlotId, model.UserId);

            return Created();
        }
    }
}
