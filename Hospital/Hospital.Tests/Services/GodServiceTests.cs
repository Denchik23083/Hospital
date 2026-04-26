using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Services.GodService;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class GodServiceTests
    {
        private readonly HospitalContext _context;
        private readonly Mock<IMapper> _mapper;
        private readonly ILogger<GodService> _logger;
        private readonly GodService _service;

        public GodServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _mapper = new Mock<IMapper>();
            _logger = Mock.Of<ILogger<GodService>>();

            _service = new GodService(_context, _mapper.Object, _logger);
        }

        [Fact]
        public async Task GetAllAdminsAsync_ShouldReturnOnlyAdmins()
        {
            var expectedCount = 1;

            var admins = new List<User>
            {
                new()
                {
                    Id = 1,
                    Email = "foo@gmail.com",
                    RoleType = RoleType.Patient,
                    PasswordHash = "hash"
                },
                new()
                {
                    Id = 2,
                    Email = "god@gmail.com",
                    RoleType = RoleType.God,
                    PasswordHash = "hash"
                },
                new()
                {
                    Id = 3,
                    Email = "admin@gmail.com",
                    RoleType = RoleType.Admin,
                    PasswordHash = "hash"
                }
            };

            await _context.Users.AddRangeAsync(admins);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAdminsAsync();

            result.Should().HaveCount(expectedCount);
            result.Should().Contain(u => u.Email == "admin@gmail.com");
            result.Should().NotContain(u => u.Email == "foo@gmail.com");
            result.Should().NotContain(u => u.Email == "god@gmail.com");
        }

        [Fact]
        public async Task GetAdminAsync_ShouldThrowUserNotFoundException_WhenAdminDoesNotExist()
        {
            var adminId = 2;

            var result = async () => await _service.GetAdminAsync(adminId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task GetAdminAsync_ShouldThrowUserNotFoundException_WhenAdminIsGod()
        {
            var adminId = 2;

            var fakeAdmin = new User
            {
                Id = adminId,
                Email = "god@gmail.com",
                RoleType = RoleType.God,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(fakeAdmin);
            await _context.SaveChangesAsync();

            var result = async () => await _service.GetAdminAsync(adminId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task GetAdminAsync_ShouldReturnAdmin_WhenAdminExists()
        {
            var adminId = 1;

            var adminDb = new User
            {
                Id = adminId,
                Email = "admin@gmail.com",
                RoleType = RoleType.Admin,
                PasswordHash = "hash"
            };

            var admin = new UserResponse
            {
                Id = adminId,
                Email = "admin@gmail.com",
            };

            await _context.Users.AddAsync(adminDb);
            await _context.SaveChangesAsync();

            _mapper
                .Setup(_ => _.Map<UserResponse>(adminDb))
                .Returns(admin);

            var result = await _service.GetAdminAsync(adminId);

            result.Should().BeEquivalentTo(admin);
        }

        [Fact]
        public async Task MakeAdminAsync_ShouldThrow_WhenUserNotFound()
        {
            var userId = 2;

            var result = async () => await _service.MakeAdminAsync(userId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task MakeAdminAsync_ShouldThrow_WhenUserIsAlreadyAdmin()
        {
            var userId = 2;

            var user = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Admin,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = async () => await _service.MakeAdminAsync(userId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task MakeAdminAsync_ShouldChangeRoleToAdmin_WhenUserExists()
        {
            var userId = 2;

            var user = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _service.MakeAdminAsync(userId);

            var updatedUser = await _context.Users.FirstAsync();

            updatedUser.RoleType.Should().Be(RoleType.Admin);
        }

        [Fact]
        public async Task MakeUserAsync_ShouldThrow_WhenAdminNotFound()
        {
            var adminId = 1;

            var result = async () => await _service.MakeUserAsync(adminId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task MakeUserAsync_ShouldThrow_WhenAdminIsAlreadyUser()
        {
            var adminId = 1;

            var admin = new User
            {
                Id = adminId,
                Email = "admin@gmail.com",
                RoleType = RoleType.Patient,
                PasswordHash = "hash"

            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            var result = async () => await _service.MakeUserAsync(adminId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task MakeUserAsync_ShouldChangeRoleToUser_WhenAdminExists()
        {
            var adminId = 1;

            var admin = new User
            {
                Id = adminId,
                Email = "admin@gmail.com",
                RoleType = RoleType.Admin,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            await _service.MakeUserAsync(adminId);

            var updatedAdmin = await _context.Users.FirstAsync();

            updatedAdmin.RoleType.Should().Be(RoleType.Patient);
        }
    }
}
