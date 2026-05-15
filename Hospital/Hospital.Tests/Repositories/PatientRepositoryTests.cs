using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.PatientRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Repositories
{
    public class PatientRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly PatientRepository _repository;

        public PatientRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new PatientRepository(_context);
        }

        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnListPatientsFromDb()
        {
            var patients = new List<Patient>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    User = new User
                    {
                        Id = 1,
                        Email = "foo@gmail.com",
                        Money = 10000m,
                        RoleType = RoleType.Patient,
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Ivan",
                    LastName = "Vanko",
                    BirthDate = new DateOnly(1990, 02, 02),
                    GenderType = GenderType.Male,
                    Phone = "+49000000",
                    User = new User
                    {
                        Id = 2,
                        Email = "too@gmail.com",
                        Money = 7000m,
                        RoleType = RoleType.Patient,
                    }
                }
            };

            var patientsResponse = new List<PatientWithUserResponse>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    User = new UserResponse
                    {
                        Email = "foo@gmail.com",
                        Money = 10000m
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Ivan",
                    LastName = "Vanko",
                    BirthDate = new DateOnly(1990, 02, 02),
                    GenderType = GenderType.Male,
                    Phone = "+49000000",
                    User = new UserResponse
                    {
                        Email = "too@gmail.com",
                        Money = 7000m
                    }
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllPatientsAsync();

            result.Should().BeEquivalentTo(patientsResponse);
        }

        [Fact]
        public async Task GetPatientAsync_ShouldReturnPatientFromDb()
        {
            var id = 2;

            var patients = new List<Patient>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    UserId = 1,
                    User = new User
                    {
                        Id = 1,
                        Email = "foo@gmail.com",
                        Money = 10000m,
                        RoleType = RoleType.Patient,
                    }
                },
                new()
                {
                    Id = id,
                    FirstName = "Ivan",
                    LastName = "Vanko",
                    BirthDate = new DateOnly(1990, 02, 02),
                    GenderType = GenderType.Male,
                    Phone = "+49000000",
                    UserId = 2,
                    User = new User
                    {
                        Id = 2,
                        Email = "too@gmail.com",
                        Money = 7000m,
                        RoleType = RoleType.Patient,
                    }
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            var result = await _repository.GetPatientAsync(id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.FirstName.Should().Be("Ivan");
            result.LastName.Should().Be("Vanko");
            result.UserId.Should().Be(2);

            result.User.Should().NotBeNull();
            result.User!.Email.Should().Be("too@gmail.com");
            result.User.Money.Should().Be(7000m);
            result.User.RoleType.Should().Be(RoleType.Patient);
        }

        [Fact]
        public async Task GetPatientByUserAsync_ShouldReturnPatientByUserFromDb()
        {
            var userId = 1;

            var patients = new List<Patient>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    UserId = userId,
                    User = new User
                    {
                        Id = userId,
                        Email = "foo@gmail.com",
                        Money = 10000m,
                        RoleType = RoleType.Patient,
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Ivan",
                    LastName = "Vanko",
                    BirthDate = new DateOnly(1990, 02, 02),
                    GenderType = GenderType.Male,
                    Phone = "+49000000",
                    UserId = 2,
                    User = new User
                    {
                        Id = 2,
                        Email = "too@gmail.com",
                        Money = 7000m,
                        RoleType = RoleType.Patient,
                    }
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            var result = await _repository.GetPatientByUserAsync(userId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.FirstName.Should().Be("Denys");
            result.LastName.Should().Be("Stark");
            result.UserId.Should().Be(userId);

            result.User.Should().NotBeNull();
            result.User!.Email.Should().Be("foo@gmail.com");
            result.User.Money.Should().Be(10000m);
            result.User.RoleType.Should().Be(RoleType.Patient);
        }

        [Fact]
        public async Task GetPatientBalanceAsync_ShouldReturnDecimalPriceFromDb()
        {
            var userId = 1;
            var balance = 10000m;

            var patients = new List<Patient>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    UserId = userId,
                    User = new User
                    {
                        Id = userId,
                        Email = "foo@gmail.com",
                        Money = balance,
                        RoleType = RoleType.Patient,
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Ivan",
                    LastName = "Vanko",
                    BirthDate = new DateOnly(1990, 02, 02),
                    GenderType = GenderType.Male,
                    Phone = "+49000000",
                    UserId = 2,
                    User = new User
                    {
                        Id = 2,
                        Email = "too@gmail.com",
                        Money = 7000m,
                        RoleType = RoleType.Patient,
                    }
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            var result = await _repository.GetPatientBalanceAsync(userId);

            result.Should().Be(balance);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldDeletePatientFromDb()
        {
            var id = 2;

            var patients = new List<Patient>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    UserId = 1,
                    User = new User
                    {
                        Id = 1,
                        Email = "foo@gmail.com",
                        Money = 10000m,
                        RoleType = RoleType.Patient,
                    }
                },
                new()
                {
                    Id = id,
                    FirstName = "Ivan",
                    LastName = "Vanko",
                    BirthDate = new DateOnly(1990, 02, 02),
                    GenderType = GenderType.Male,
                    Phone = "+49000000",
                    UserId = 2,
                    User = new User
                    {
                        Id = 2,
                        Email = "too@gmail.com",
                        Money = 7000m,
                        RoleType = RoleType.Patient,
                    }
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            var patientToDelete = await _context.Patients
                .FirstOrDefaultAsync(_ => _.Id == id);

            await _repository.DeletePatientAsync(patientToDelete!);
            await _context.SaveChangesAsync();

            var result = await _context.Patients
                .FirstOrDefaultAsync(_ => _.Id == id);

            result.Should().BeNull();

            var patientsCount = await _context.Patients.CountAsync();

            patientsCount.Should().Be(1);
        }
    }
}
