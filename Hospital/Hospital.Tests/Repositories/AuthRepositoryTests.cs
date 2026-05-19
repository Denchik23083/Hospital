using FluentAssertions;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Repositories
{
    public class AuthRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly AuthRepository _repository;

        public AuthRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new AuthRepository(_context);
        }

        [Fact]
        public async Task IsEmailNotUniqueAsync_ShouldReturnTrue_WhenEmailExists()
        {
            var email = "foo@gmail.com";

            var user = new User
            {
                Email = "foo@gmail.com",
                PasswordHash = "0000",
                RoleType = RoleType.Patient
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _repository.IsEmailNotUniqueAsync(email);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsEmailNotUniqueAsync_ShouldReturnFalse_WhenEmailNotExists()
        {
            var email = "too@gmail.com";

            var user = new User
            {
                Email = "foo@gmail.com",
                PasswordHash = "0000",
                RoleType = RoleType.Patient
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _repository.IsEmailNotUniqueAsync(email);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUserByEmailFromDb()
        {
            var email = "foo@gmail.com";

            var users = new List<User>
            {
                new()
                {
                    Id = 1,
                    Money = 1000m,
                    Patient = new Patient
                    {
                        Id = 1,
                        FirstName = "Foo",
                        LastName = "Too"
                    },
                    Email = email,
                    RoleType = RoleType.Patient,
                    RefreshToken = "refresh-token",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
                },
                new()
                {
                    Id = 2,
                    Money = 700m,
                    Doctor = new Doctor
                    {
                        Id = 1,
                        FirstName = "Too",
                        LastName = "Foo"
                    },
                    Email = "too@gmail.com",
                    RoleType = RoleType.Doctor,
                    RefreshToken = "refresh-token",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var result = await _repository.GetUserByEmailAsync(email);

            result.Should().NotBeNull();

            result.Email.Should().Be(email);
            result.Money.Should().Be(1000m);
            result.RoleType.Should().Be(RoleType.Patient);
      
            result.Patient.Should().NotBeNull();
            result.Patient.FirstName.Should().Be(result.Patient.FirstName);
            result.Patient.LastName.Should().Be(result.Patient.LastName);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUserFromDb()
        {
            var id = 2;

            var users = new List<User>
            {
                new()
                {
                    Id = 1,
                    Money = 1000m,
                    Patient = new Patient
                    {
                        Id = 1,
                        FirstName = "Foo",
                        LastName = "Too"
                    },
                    Email = "foo@gmail.com",
                    RoleType = RoleType.Patient,
                    RefreshToken = "refresh-token",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
                },
                new()
                {
                    Id = id,
                    Money = 700m,
                    Doctor = new Doctor
                    {
                        Id = 1,
                        FirstName = "Too",
                        LastName = "Foo"
                    },
                    Email = "too@gmail.com",
                    RoleType = RoleType.Doctor,
                    RefreshToken = "refresh-token",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var result = await _repository.GetUserAsync(id);

            result.Should().NotBeNull();

            result.Email.Should().Be("too@gmail.com");
            result.Money.Should().Be(700m);
            result.RoleType.Should().Be(RoleType.Doctor);

            result.Doctor.Should().NotBeNull();
            result.Doctor.FirstName.Should().Be(result.Doctor.FirstName);
            result.Doctor.LastName.Should().Be(result.Doctor.LastName);
        }

        [Fact]
        public async Task RegisterAsync_ShouldAddUserToDb()
        {
            var register = new User
            {
                PasswordHash = "0000",
                Patient = new Patient
                {
                    FirstName = "Foo",
                    LastName = "Too",
                    BirthDate = new DateOnly(2001, 01, 02),
                    GenderType = GenderType.Male,
                    Phone = "497777777",
                },
                Email = "foo@gmail.com",
                RoleType = RoleType.Patient,
            };

            await _context.Users.AddAsync(register);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FirstOrDefaultAsync();

            user.Should().NotBeNull();
            user.PasswordHash.Should().Be(register.PasswordHash);
            user.Email.Should().Be(register.Email);
            user.RoleType.Should().Be(register.RoleType);

            user.Patient.Should().NotBeNull();
            user.Patient.FirstName.Should().Be(register.Patient.FirstName);
            user.Patient.LastName.Should().Be(register.Patient.LastName);
            user.Patient.BirthDate.Should().Be(register.Patient.BirthDate);
            user.Patient.GenderType.Should().Be(register.Patient.GenderType);
            user.Patient.Phone.Should().Be(register.Patient.Phone);
        }
    }
}
