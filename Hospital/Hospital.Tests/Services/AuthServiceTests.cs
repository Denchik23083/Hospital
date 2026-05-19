using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.AuthService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace Hospital.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _repository;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _repository = new Mock<IAuthRepository>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();
            _configuration = TestConfigurationFactory.Create();
            _logger = Mock.Of<ILogger<AuthService>>();

            _service = new AuthService(_repository.Object, 
                _unitOfWorkRepository.Object, _configuration, _logger);
        }

        //Throw Exception Condition
        [Fact]
        public async Task RegisterAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var register = new RegisterRequest(
                "foo@gmail.com", "0000",
                "Denys", "Kudriavov", "497777777",
                new DateOnly(2003, 01, 01), GenderType.Male);

            _repository
                .Setup(_ => _.IsEmailNotUniqueAsync(register.Email))
                .ReturnsAsync(true);

            var action = async () => await _service.RegisterAsync(register);

            await action.Should().ThrowAsync<ConflictException>();

            _repository.Verify(_ => _.IsEmailNotUniqueAsync(register.Email), Times.Once);

            _repository.Verify(_ => _.RegisterAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserNotFound()
        {
            var login = new LoginRequest("foo@gmail.com", "0000");

            _repository
                .Setup(_ => _.GetUserByEmailAsync(login.Email))
                .ReturnsAsync((User?)null);

            var action = async () => await _service.LoginAsync(login);

            await action.Should().ThrowAsync<UnauthorizedException>();

            _repository.Verify(_ => _.GetUserByEmailAsync(login.Email), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsWrong()
        {
            var user = new User
            {
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            var login = new LoginRequest("foo@gmail.com", "1111");

            _repository
                .Setup(_ => _.GetUserByEmailAsync(login.Email))
                .ReturnsAsync(user);

            var action = async () => await _service.LoginAsync(login);

            await action.Should().ThrowAsync<UnauthorizedException>();

            _repository.Verify(_ => _.GetUserByEmailAsync(login.Email), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenUserNotFound()
        {
            var userId = 999;

            var refresh = new RefreshTokenRequest(userId, "refresh-token");

            _repository
                .Setup(_ => _.GetUserAsync(userId))
                .ReturnsAsync((User?)null);

            var action = async () => await _service.RefreshTokenAsync(refresh);

            await action.Should().ThrowAsync<UnauthorizedException>();

            _repository.Verify(_ => _.GetUserAsync(userId), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenRefreshTokenDoesNotMatch()
        {
            var userId = 1;
            var fakeRefreshToken = "fake-refresh-token";

            var user = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                RefreshToken = "refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            _repository
                .Setup(_ => _.GetUserAsync(userId))
                .ReturnsAsync(user);

            var refresh = new RefreshTokenRequest(user.Id, fakeRefreshToken);

            var action = async () => await _service.RefreshTokenAsync(refresh);

            await action.Should().ThrowAsync<UnauthorizedException>();

            _repository.Verify(_ => _.GetUserAsync(userId), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenRefreshTokenExpired()
        {
            var userId = 1;

            var user = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                RefreshToken = "refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            _repository
                .Setup(_ => _.GetUserAsync(userId))
                .ReturnsAsync(user);

            var refresh = new RefreshTokenRequest(user.Id, "refresh-token");

            var action = async () => await _service.RefreshTokenAsync(refresh);

            await action.Should().ThrowAsync<UnauthorizedException>();

            _repository.Verify(_ => _.GetUserAsync(userId), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        //Method
        [Fact]
        public async Task RegisterAsync_ShouldAddUserWithHashedPasswordAndUserRole_WhenDataIsValid()
        {
            var register = new RegisterRequest(
                "foo@gmail.com", "0000",
                "Denys", "Kudriavov", "497777777",
                new DateOnly(2003, 01, 01), GenderType.Male);

            User? user = null;

            _repository
                .Setup(_ => _.IsEmailNotUniqueAsync(register.Email))
                .ReturnsAsync(false);

            _repository
                .Setup(_ => _.RegisterAsync(It.IsAny<User>()))
                .Callback<User>(u => user = u)
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.RegisterAsync(register);

            user.Should().NotBeNull();

            user.Email.Should().Be(register.Email);
            user.RoleType.Should().Be(RoleType.Patient);

            user.Patient.Should().NotBeNull();
            user.Patient.FirstName.Should().Be(register.FirstName);
            user.Patient.LastName.Should().Be(register.LastName);
            user.Patient.BirthDate.Should().Be(register.BirthDate);
            user.Patient.GenderType.Should().Be(register.GenderType);
            user.Patient.Phone.Should().Be(register.Phone);

            user.PasswordHash.Should().NotBeNullOrWhiteSpace();
            user.PasswordHash.Should().NotBe(register.Password);

            var verifyResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, register.Password);

            verifyResult.Should().Be(PasswordVerificationResult.Success);

            _repository.Verify(_ => _.IsEmailNotUniqueAsync(register.Email), Times.Once);
            _repository.Verify(_ => _.RegisterAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokensAndUpdateRefreshToken_WhenCredentialsAreValid()
        {
            var user = new User
            {
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            var login = new LoginRequest("foo@gmail.com", "0000");

            _repository
                .Setup(_ => _.GetUserByEmailAsync(login.Email))
                .ReturnsAsync(user);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.LoginAsync(login);

            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();

            user.RefreshToken.Should().Be(result.RefreshToken);
            user.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);

            _repository.Verify(_ => _.GetUserByEmailAsync(login.Email), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            var userId = 1;

            var user = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                RefreshToken = "refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            _repository
                .Setup(_ => _.GetUserAsync(userId))
                .ReturnsAsync(user);

            var oldRefreshToken = user.RefreshToken;

            var refresh = new RefreshTokenRequest(user.Id, oldRefreshToken);

            var result = await _service.RefreshTokenAsync(refresh);

            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBe(oldRefreshToken);

            user.RefreshToken.Should().Be(result.RefreshToken);
            user.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);

            _repository.Verify(_ => _.GetUserAsync(userId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }
    }
}
