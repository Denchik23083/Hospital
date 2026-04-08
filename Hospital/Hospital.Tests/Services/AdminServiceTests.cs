using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Responce;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Services.AdminService;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly HospitalContext _context;
        private readonly Mock<IMapper> _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _mapper = new Mock<IMapper>();
            _logger = Mock.Of<ILogger<AdminService>>();

            _service = new AdminService(_context, _mapper.Object, _logger);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnOnlyUsers()
        {
            var expectedCount = 1;

            var users = new List<User>
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

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllUsersAsync();

            result.Should().HaveCount(expectedCount);
            result.Should().Contain(u => u.Email == "foo@gmail.com");
            result.Should().NotContain(u => u.Email == "admin@gmail.com");
            result.Should().NotContain(u => u.Email == "god@gmail.com");
        }

        [Fact]
        public async Task GetUserAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
        {
            var userId = 2;

            var result = async () => await _service.GetUserAsync(userId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }
        
        [Fact]
        public async Task GetUserAsync_ShouldThrowUserNotFoundException_WhenUserIsAdmin()
        {
            var userId = 2;

            var fakeUser = new User
            {
                Id = userId,
                Email = "god@gmail.com",
                RoleType = RoleType.God,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(fakeUser);
            await _context.SaveChangesAsync();

            var result = async () => await _service.GetUserAsync(userId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
        {
            var userId = 1;

            var userDb = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                PasswordHash = "hash"
            };

            var user = new UserResponce
            {
                Id = userId,
                Email = "foo@gmail.com",
            };

            await _context.Users.AddAsync(userDb);
            await _context.SaveChangesAsync();

            _mapper
                .Setup(_ => _.Map<UserResponce>(userDb))
                .Returns(user);

            var result = await _service.GetUserAsync(userId);

            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
        {
            var userId = 2;

            var result = async () => await _service.DeleteUserAsync(userId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowUserNotFoundException_WhenUserIsAdmin()
        {
            var userId = 2;

            var fakeUser = new User
            {
                Id = userId,
                Email = "god@gmail.com",
                RoleType = RoleType.God,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(fakeUser);
            await _context.SaveChangesAsync();

            var result = async () => await _service.DeleteUserAsync(userId);

            await result.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnUser_WhenUserExists()
        {
            var userId = 1;

            var userToDelete = new User
            {
                Id = userId,
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
                PasswordHash = "hash"
            };

            await _context.Users.AddAsync(userToDelete);
            await _context.SaveChangesAsync();

            await _service.DeleteUserAsync(userId);

            var deletedUser = await _context.Users.AnyAsync();

            deletedUser.Should().BeFalse();
        }
    }
}
