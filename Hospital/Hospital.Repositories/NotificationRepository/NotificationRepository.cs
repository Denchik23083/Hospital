using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.NotificationRepository
{
    public class NotificationRepository(HospitalContext context) : INotificationRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(int userId, CancellationToken ct)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task<Notification?> GetNotificationAsync(int id, int userId, CancellationToken ct)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(_ => _.Id == id && _.UserId == userId, ct);
        }

        public async Task AddNotificationAsync(Notification notification, CancellationToken ct)
        {
            await _context.Notifications.AddAsync(notification, ct);
        }

        public Task DeleteNotificationAsync(Notification notification, CancellationToken ct = default)
        {
            _context.Notifications.Remove(notification);

            return Task.CompletedTask;
        }
    }
}
