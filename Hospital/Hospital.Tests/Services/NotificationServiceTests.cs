using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.NotificationService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _repository;
        private readonly ILogger<NotificationService> _logger;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _repository = new Mock<INotificationRepository>();
            _logger = Mock.Of<ILogger<NotificationService>>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();

            _service = new NotificationService(_repository.Object,
                _logger, _unitOfWorkRepository.Object);
        }

        [Fact]
        public async Task GetAllNotificationsAsync_ShouldReturnListNotifications()
        {
            var userId = 10;

            var notifications = new List<NotificationResponse>
            {
                new()
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Message = "Привет"
                },
                new()
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Message = "Мир"
                }
            };

            _repository
                .Setup(_ => _.GetAllNotificationsAsync(userId))
                .ReturnsAsync(notifications);

            var result = await _service.GetAllNotificationsAsync(userId);

            result.Should().BeEquivalentTo(notifications);

            _repository.Verify(_ => _.GetAllNotificationsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetNotificationAsync_ShouldThrowNotFoundException_Logger()
        {
            var id = 1;
            var userId = 10;

            _repository
                .Setup(_ => _.GetNotificationAsync(id, userId))
                .ReturnsAsync((Notification?)null);

            var action = async () => await _service.DeleteNotificationAsync(id, userId);

            await action.Should().ThrowAsync<NotificationNotFoundException>();

            _repository.Verify(_ => _.GetNotificationAsync(id, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteNotificationAsync_ShouldDeleteNotification_WhenNotificationExists()
        {
            var id = 1;
            var userId = 10;

            var notificationToDelete = new Notification
            {
                Id = id,
                CreatedAt = DateTime.UtcNow.AddMonths(-1),
                Message = "Hello world!",
                UserId = userId
            };

            _repository
                .Setup(_ => _.GetNotificationAsync(id, userId))
                .ReturnsAsync(notificationToDelete);

            _repository
                .Setup(_ => _.DeleteNotificationAsync(notificationToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.DeleteNotificationAsync(id, userId);

            _repository.Verify(_ => _.GetNotificationAsync(id, userId), Times.Once);
            _repository.Verify(_ => _.DeleteNotificationAsync(notificationToDelete), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }
    }
}
