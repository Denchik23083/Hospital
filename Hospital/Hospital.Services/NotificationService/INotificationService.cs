using Hospital.Core.Models.Response;

namespace Hospital.Services.NotificationService
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponse>> GetAllNotificationsAsync(int userId, CancellationToken ct);

        Task DeleteNotificationAsync(int id, int userId, CancellationToken ct);
    }
}