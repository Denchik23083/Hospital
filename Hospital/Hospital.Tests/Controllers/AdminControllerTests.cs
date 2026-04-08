using FluentAssertions;
using Hospital.AdminPanel.Controllers;
using Hospital.Core.Models.Responce;
using Hospital.Services.AdminService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _service;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _service = new Mock<IAdminService>();
            _controller = new AdminController(_service.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnOkWithUsers()
        {
            var users = new List<UserResponce>
            {
                new()
                {
                    Id = 1,
                    Email = "foo@gmail.com",
                },
                new()
                {
                    Id = 2,
                    Email = "fff@gmail.com",
                }
            };

            _service
                .Setup(_ => _.GetAllUsersAsync())
                .ReturnsAsync(users);

            var result = await _controller.GetAllUsersAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(users);

            _service.Verify(_ => _.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnOkWithUser()
        {
            var userId = 1;

            var user = new UserResponce
            {
                Id = userId,
                Email = "foo@gmail.com",
            };

            _service
                .Setup(_ => _.GetUserAsync(userId))
                .ReturnsAsync(user);

            var result = await _controller.GetUserAsync(userId);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().Be(user);

            _service.Verify(_ => _.GetUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnNoContent()
        {
            var userId = 1;

            _service
                .Setup(_ => _.DeleteUserAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteUserAsync(userId);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.DeleteUserAsync(userId), Times.Once);
        }
    }
}
