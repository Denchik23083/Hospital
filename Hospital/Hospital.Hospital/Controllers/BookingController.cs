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
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAllPatientBookingsAsync(CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bookings = await _service.GetAllPatientBookingsAsync(userId, ct);

            return Ok(bookings);
        }

        [HttpPost("{slotId}")]
        [Authorize(Roles = AppRoles.Patient)]
        public async Task<ActionResult> CreateBookingAsync(int slotId, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.CreateBookingAsync(slotId, userId, ct);

            return Created();
        }

        [HttpPut("{id}/complete")]
        [Authorize(Roles = AppRoles.Doctor)]
        public async Task<ActionResult> CompleteBookingAsync(int id, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.CompleteBookingAsync(id, userId, ct);

            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = AppRoles.Patient)]
        public async Task<ActionResult> CancelBookingAsync(int id, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.CancelBookingAsync(id, userId, ct);

            return NoContent();
        }
    }
}
