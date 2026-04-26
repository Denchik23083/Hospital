using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Services.BookingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class BookingController(IBookingService service) : ControllerBase
    {
        private readonly IBookingService _service = service;

        [Authorize(Roles = AppRoles.Patient)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAllPatientBookingsAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bookings = await _service.GetAllPatientBookingsAsync(userId);

            return Ok(bookings);
        }

        [HttpPost("{slotId}")]
        [Authorize(Roles = AppRoles.Patient)]
        public async Task<ActionResult> CreateBookingAsync(int slotId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.CreateBookingAsync(slotId, userId);

            return Created();
        }
    }
}
