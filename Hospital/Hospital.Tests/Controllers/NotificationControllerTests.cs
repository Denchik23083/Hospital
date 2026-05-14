using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Hospital.Controllers;
using Hospital.Services.NotificationService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class NotificationControllerTests
    {
        private readonly Mock<INotificationService> _service;
        private readonly NotificationController _controller;

        public NotificationControllerTests()
        {
            _service = new Mock<INotificationService>();
            _controller = new NotificationController(_service.Object);
        }

        [Fact]
        public async Task GetAllNotificationsAsync_ShouldReturnOk_WithListNotifications()
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

            _service
                .Setup(_ => _.GetAllNotificationsAsync(userId))
                .ReturnsAsync(notifications);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllNotificationsAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(notifications);

            _service.Verify(_ => _.GetAllNotificationsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteNotificationAsync_ShouldReturnNoContent()
        {
            var id = 1;
            var userId = 10;

            _service
                .Setup(_ => _.DeleteNotificationAsync(id, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.DeleteNotificationAsync(id);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.DeleteNotificationAsync(id, userId), Times.Once);
        }
    }
}
