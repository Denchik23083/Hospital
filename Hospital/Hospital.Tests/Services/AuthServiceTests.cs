using Hospital.Db;
using Hospital.Services.AuthService;
using Hospital.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly HospitalContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _configuration = TestConfigurationFactory.Create();
            _logger = Mock.Of<ILogger<AuthService>>();

            _service = new AuthService(_context, _configuration, _logger);
        }

        /*[Fact]
        public async Task RegisterAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var user = new User
            {
                Email = "foo@gmail.com",
                PasswordHash = "0000",
                RoleType = RoleType.Patient
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var register = new RegisterRequest("Foo", "foo@gmail.com", "0000");

            var result = async () => await _service.RegisterAsync(register);

            await result.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task RegisterAsync_ShouldAddUserWithHashedPasswordAndUserRole_WhenDataIsValid()
        {
            var register = new RegisterRequest("Foo", "foo@gmail.com", "0000");

            var mappedUser = new User
            {
                Email = register.Email,
            };

            _mapper
                .Setup(_ => _.Map<User>(register))
                .Returns(mappedUser);

            await _service.RegisterAsync(register);

            var savedUser = await _context.Users.SingleAsync(_ => _.Email == register.Email);

            savedUser.Email.Should().Be(register.Email);
            savedUser.RoleType.Should().Be(RoleType.Patient);
            savedUser.PasswordHash.Should().NotBeNullOrWhiteSpace();
            savedUser.PasswordHash.Should().NotBe(register.Password);

            var verifyResult = new PasswordHasher<User>()
                .VerifyHashedPassword(savedUser, savedUser.PasswordHash, register.Password);

            verifyResult.Should().Be(PasswordVerificationResult.Success);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserNotFound()
        {
            var login = new LoginRequest("foo@gmail.com", "0000");

            var result = async () => await _service.LoginAsync(login);

            await result.Should().ThrowAsync<UnauthorizedException>();
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

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new LoginRequest("foo@gmail.com", "1111");

            var result = async () => await _service.LoginAsync(login);

            await result.Should().ThrowAsync<UnauthorizedException>();
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

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new LoginRequest("foo@gmail.com", "0000");

            var result = await _service.LoginAsync(login);

            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();

            var updatedUser = await _context.Users.SingleAsync(_ => _.Email == login.Email);

            updatedUser.RefreshToken.Should().Be(result.RefreshToken);
            updatedUser.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenUserNotFound()
        {
            var refresh = new RefreshTokenRequest(999, "refresh-token");

            var result = async () => await _service.RefreshTokenAsync(refresh);

            await result.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenRefreshTokenDoesNotMatch()
        {
            var user = new User
            {
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                RefreshToken = "refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var refresh = new RefreshTokenRequest(user.Id, "fake-refresh-token");

            var result = async () => await _service.RefreshTokenAsync(refresh);

            await result.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenRefreshTokenExpired()
        {
            var user = new User
            {
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                RefreshToken = "refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var refresh = new RefreshTokenRequest(user.Id, "refresh-token");

            var result = async () => await _service.RefreshTokenAsync(refresh);

            await result.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            var user = new User
            {
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                RefreshToken = "refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, "0000");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var oldRefreshToken = user.RefreshToken;

            var refresh = new RefreshTokenRequest(user.Id, oldRefreshToken);

            var result = await _service.RefreshTokenAsync(refresh);

            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBe(oldRefreshToken);

            var updatedUser = await _context.Users.SingleAsync(_ => _.Email == user.Email);

            updatedUser.RefreshToken.Should().Be(result.RefreshToken);
            updatedUser.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);
        }*/
    }
}
