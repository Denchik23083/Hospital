using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.NotificationService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _repository;
        private readonly ILogger<NotificationService> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _repository = new Mock<INotificationRepository>();
            _logger = Mock.Of<ILogger<NotificationService>>();
            _mapper = new Mock<IMapper>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();

            _service = new NotificationService(_repository.Object,
                _logger, _mapper.Object, _unitOfWorkRepository.Object);
        }

        //Throw Exception Condition
        [Fact]
        public async Task DeleteNotificationAsync_ShouldThrowNotificationNotFoundException_Logger()
        {
            var id = 1;
            var userId = 10;

            _repository
                .Setup(_ => _.GetNotificationAsync(id, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notification?)null);

            var action = async () => await _service.DeleteNotificationAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<NotificationNotFoundException>();

            _repository.Verify(_ => _.GetNotificationAsync(id, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        //Method
        [Fact]
        public async Task GetAllNotificationsAsync_ShouldReturnListNotifications()
        {
            var userId = 10;

            var notifications = new List<Notification>
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

            var notificationsResponse = new List<NotificationResponse>
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
                .Setup(_ => _.GetAllNotificationsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);

            _mapper
                .Setup(_ => _.Map<IEnumerable<NotificationResponse>>(notifications))
                .Returns(notificationsResponse);

            var result = await _service.GetAllNotificationsAsync(userId, CancellationToken.None);

            result.Should().BeEquivalentTo(notificationsResponse);

            _repository.Verify(_ => _.GetAllNotificationsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<NotificationResponse>>(notifications));
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
                .Setup(_ => _.GetNotificationAsync(id, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notificationToDelete);

            _repository
                .Setup(_ => _.DeleteNotificationAsync(notificationToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.DeleteNotificationAsync(id, userId, CancellationToken.None);

            _repository.Verify(_ => _.GetNotificationAsync(id, userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.DeleteNotificationAsync(notificationToDelete), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }
    }
}
