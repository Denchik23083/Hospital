using Hospital.Core.Models.Response;
using Hospital.Db.Entities;

namespace Hospital.Repositories.NotificationRepository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<NotificationResponce>> GetAllNotificationsAsync(int userId);
        
        Task<Notification?> GetNotificationAsync(int id, int userId);

        Task AddNotificationAsync(Notification notification);

        Task DeleteNotificationAsync(Notification notification);
    }
}