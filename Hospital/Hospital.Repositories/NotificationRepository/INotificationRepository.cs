using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.NotificationRepository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllNotificationsAsync(int userId, CancellationToken ct);
        
        Task<Notification?> GetNotificationAsync(int id, int userId, CancellationToken ct);

        Task AddNotificationAsync(Notification notification, CancellationToken ct);

        Task DeleteNotificationAsync(Notification notification, CancellationToken ct);
    }
}