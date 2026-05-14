using Hospital.Core.Models.Response;
using Hospital.Core.Utilities;
using Hospital.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Hospital.Controllers
{
    [Route("api/hospital/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        private readonly INotificationService _service = service;

        [Authorize(Roles = AppRoles.DoctorPatient)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetAllNotificationsAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications = await _service.GetAllNotificationsAsync(userId);

            return Ok(notifications);
        }

        [Authorize(Roles = AppRoles.DoctorPatient)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotificationAsync(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.DeleteNotificationAsync(id, userId);

            return NoContent();
        }
    }
}
