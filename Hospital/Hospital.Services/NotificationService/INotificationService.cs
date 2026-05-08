using Hospital.Core.Models.Response;

namespace Hospital.Services.NotificationService
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponce>> GetAllNotificationsAsync(int userId);

        Task DeleteNotificationAsync(int id, int userId);
    }
}