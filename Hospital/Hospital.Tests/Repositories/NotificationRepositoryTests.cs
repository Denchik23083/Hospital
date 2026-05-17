using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Repositories.NotificationRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Repositories
{
    public class NotificationRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly NotificationRepository _repository;

        public NotificationRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new NotificationRepository(_context);
        }

        [Fact]
        public async Task GetAllNotificationsAsync_ShouldReturnListNotificationsFromDb()
        {
            var userId = 10;
            var date1 = DateTime.UtcNow.AddDays(-1);
            var date2 = DateTime.UtcNow.AddDays(-2);

            var notifications = new List<Notification>
            {
                new()
                {
                    Id = 1,
                    CreatedAt = date1,
                    Message = "Привет",
                    UserId = userId
                },
                new()
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Message = "Привет",
                    UserId = 1
                },
                new()
                {
                    Id = 3,
                    CreatedAt = date2,
                    Message = "Мир",
                    UserId = userId
                }
            };

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            var notificationsResponse = new List<NotificationResponse>
            {
                new()
                {
                    Id = 1,
                    CreatedAt = date1,
                    Message = "Привет"
                },
                new()
                {
                    Id = 3,
                    CreatedAt = date2,
                    Message = "Мир"
                }
            };

            var result = await _repository.GetAllNotificationsAsync(userId);

            result.Should().BeEquivalentTo(notificationsResponse);
        }

        [Fact]
        public async Task GetNotificationAsync_ShouldReturnNotificationFromDb()
        {
            var id = 1;
            var userId = 10;
            var date1 = DateTime.UtcNow.AddDays(-1);

            var notifications = new List<Notification>
            {
                new()
                {
                    Id = 1,
                    CreatedAt = date1,
                    Message = "Привет",
                    UserId = userId
                },
                new()
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Message = "Привет",
                    UserId = 1
                },
                new()
                {
                    Id = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Message = "Мир",
                    UserId = userId
                }
            };

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            var notificationResponse = new NotificationResponse
            {
                Id = 1,
                CreatedAt = date1,
                Message = "Привет"
            };

            var result = await _repository.GetNotificationAsync(id, userId);

            result.Should().BeEquivalentTo(notificationResponse);
        }

        [Fact]
        public async Task AddNotificationAsync_ShouldAddNotificationToDb()
        {
            var notification = new Notification
            {
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Message = "Привет",
                UserId = 10
            };

            await _repository.AddNotificationAsync(notification);
            await _context.SaveChangesAsync();

            var result = await _context.Notifications.FirstOrDefaultAsync();

            result.Should().NotBeNull();
            result.Message.Should().Be(notification.Message);
            result.UserId.Should().Be(notification.UserId);
        }

        [Fact]
        public async Task DeleteNotificationAsync_ShouldDeleteNotificationFromDb()
        {
            var id = 1;

            var notifications = new List<Notification>
            {
                new()
                {
                    Id = id,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Message = "Привет",
                    UserId = 12
                },
                new()
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Message = "Привет",
                    UserId = 1
                },
                new()
                {
                    Id = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Message = "Мир",
                    UserId = 4
                }
            };

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            var notificationToDelete = await _context.Notifications
                .FirstOrDefaultAsync(_ => _.Id == id);

            await _repository.DeleteNotificationAsync(notificationToDelete!);
            await _context.SaveChangesAsync();

            var result = await _context.Notifications
                .FirstOrDefaultAsync(_ => _.Id == id);

            result.Should().BeNull();

            var notificationsCount = await _context.Notifications.CountAsync();

            notificationsCount.Should().Be(2);
        }
    }
}
