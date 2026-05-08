using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.NotificationRepository
{
    public class NotificationRepository(HospitalContext context) : INotificationRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<NotificationResponce>> GetAllNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Where(_ => _.UserId == userId)
                .Select(_ => new NotificationResponce
                {
                    Id = _.Id,
                    CreatedAt = _.CreatedAt,
                    Message = _.Message
                }).ToListAsync();
        }

        public async Task<Notification?> GetNotificationAsync(int id, int userId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(_ => _.Id == id && _.UserId == userId);
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public Task DeleteNotificationAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);

            return Task.CompletedTask;
        }
    }
}
