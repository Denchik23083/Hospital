using FluentAssertions;
using Hospital.AdminPanel.Controllers;
using Hospital.Core.Models.Response;
using Hospital.Services.GodService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class GodControllerTests
    {
        private readonly Mock<IGodService> _service;
        private readonly GodController _controller;

        public GodControllerTests()
        {
            _service = new Mock<IGodService>();
            _controller = new GodController(_service.Object);
        }

        [Fact]
        public async Task GetAllAdminsAsync_ShouldReturnOkWithAdmins()
        {
            var admins = new List<UserResponse>
            {
                new()
                {
                    Id = 1,
                    Email = "admin@gmail.com",
                },
                new()
                {
                    Id = 2,
                    Email = "fff@gmail.com",
                }
            };

            _service
                .Setup(_ => _.GetAllAdminsAsync())
                .ReturnsAsync(admins);

            var result = await _controller.GetAllAdminsAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(admins);

            _service.Verify(_ => _.GetAllAdminsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnOkWithUser()
        {
            var adminId = 1;

            var admin = new UserResponse
            {
                Id = adminId,
                Email = "admin@gmail.com",
            };

            _service
                .Setup(_ => _.GetAdminAsync(adminId))
                .ReturnsAsync(admin);

            var result = await _controller.GetAdminAsync(adminId);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().Be(admin);

            _service.Verify(_ => _.GetAdminAsync(adminId), Times.Once);
        }

        [Fact]
        public async Task MakeAdminAsync_ShouldReturnNoContent()
        {
            var userId = 1;

            _service
                .Setup(_ => _.MakeAdminAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.MakeAdminAsync(userId);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.MakeAdminAsync(userId), Times.Once);
        }

        [Fact]
        public async Task MakeUserAsync_ShouldReturnNoContent()
        {
            var adminId = 1;

            _service
                .Setup(_ => _.MakeUserAsync(adminId))
                .Returns(Task.CompletedTask);

            var result = await _controller.MakeUserAsync(adminId);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.MakeUserAsync(adminId), Times.Once);
        }
    }
}
