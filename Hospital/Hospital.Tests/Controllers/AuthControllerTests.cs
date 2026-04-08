using FluentAssertions;
using Hospital.Auth.Controllers;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Responce;
using Hospital.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _service;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _service = new Mock<IAuthService>();
            _controller = new AuthController(_service.Object);
        }

        /*[Fact]
        public async Task RegisterAsync_ShouldReturnCreated_WhenRegistrationSuccessful()
        {
            var register = new RegisterRequest("Foo", "foo@gmail.com", "0000");

            _service
                .Setup(s => s.RegisterAsync(register))
                .Returns(Task.CompletedTask);

            var result = await _controller.RegisterAsync(register);

            result.Should().BeOfType<CreatedResult>();

            _service.Verify(_ => _.RegisterAsync(register), Times.Once);
        }*/

        [Fact]
        public async Task LoginAsync_ShouldReturnOkWithToken_WhenLoginSuccessful()
        {
            var login = new LoginRequest("foo@gmail.com", "0000");

            var tokenResponce = new TokenResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            _service
                .Setup(s => s.LoginAsync(login))
                .ReturnsAsync(tokenResponce);

            var result = await _controller.LoginAsync(login);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().Be(tokenResponce);

            _service.Verify(_ => _.LoginAsync(login), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnOkWithToken_WhenRefreshSuccessful()
        {
            var refresh = new RefreshTokenRequest(1, "refresh-token");

            var tokenResponce = new TokenResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            _service
                .Setup(s => s.RefreshTokenAsync(refresh))
                .ReturnsAsync(tokenResponce);

            var result = await _controller.RefreshTokenAsync(refresh);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().Be(tokenResponce);

            _service.Verify(_ => _.RefreshTokenAsync(refresh), Times.Once);
        }
    }
}
