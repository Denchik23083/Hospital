using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.DoctorRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Repositories
{
    public class DoctorRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly DoctorRepository _repository;

        public DoctorRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new DoctorRepository(_context);
        }

        [Fact]
        public async Task GetAllDoctorsAsync_ShouldReturnListDoctorsFromDb()
        {
            var doctors = new List<Doctor>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    ExperienceYears = 4,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new Specialty
                    {
                        Id = 2,
                        Name = "Кардиология",
                        Price = 80
                    },
                    User = new User
                    {
                        Id = 1,
                        Email = "doctor24@gmail.com",
                        Money = 100m
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },
                    User = new User
                    {
                        Id = 2,
                        Email = "doctor1@gmail.com",
                        Money = 500m
                    }
                },
                new()
                {
                    Id = 3,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 2,
                    GenderType = GenderType.Female,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    SpecialtyId = 1,
                    User = new User
                    {
                        Id = 5,
                        Email = "doctor4@gmail.com",
                        Money = 400m
                    }
                }
            };

            var doctorsResponse = new List<DoctorWithUserResponse>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    ExperienceYears = 4,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new SpecialtyResponse
                    {
                        Id = 2,
                        Name = "Кардиология",
                        Price = 80
                    },
                    User = new UserResponse
                    {
                        Email = "doctor24@gmail.com",
                        Money = 100m
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new SpecialtyResponse
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },
                    User = new UserResponse
                    {
                        Email = "doctor1@gmail.com",
                        Money = 500m
                    }
                },
                new()
                {
                    Id = 3,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 2,
                    GenderType = GenderType.Female,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new SpecialtyResponse
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },
                    User = new UserResponse
                    {
                        Email = "doctor4@gmail.com",
                        Money = 400m
                    }
                }
            };

            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllDoctorsAsync();

            result.Should().BeEquivalentTo(doctorsResponse);
        }

        [Fact]
        public async Task GetAllDoctorsBySpecialtyAsync_ShouldReturnDoctorsBySpecialtyFromDb()
        {
            var specialtyId = 1;

            var doctors = new List<Doctor>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    ExperienceYears = 4,
                    GenderType = GenderType.Male,
                    SpecialtyId = 2
                },
                new()
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,
                    SpecialtyId = specialtyId
                },
                new()
                {
                    Id = 3,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 2,
                    GenderType = GenderType.Female,
                    SpecialtyId = specialtyId
                }
            };

            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            var doctorsResponse = new List<DoctorResponse>
            {
                new()
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 3,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 2,
                    GenderType = GenderType.Female
                }
            };

            var result = await _repository.GetAllDoctorsBySpecialtyAsync(specialtyId);

            result.Should().BeEquivalentTo(doctorsResponse);
        }

        [Fact]
        public async Task GetDoctorAsync_ShouldReturnDoctorFromDb()
        {
            var id = 2;

            var doctors = new List<Doctor>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    ExperienceYears = 4,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new Specialty
                    {
                        Id = 2,
                        Name = "Кардиология",
                        Price = 80
                    },
                    User = new User
                    {
                        Id = 1,
                        Email = "doctor24@gmail.com",
                        Money = 100m
                    }
                },
                new()
                {
                    Id = id,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },
                    User = new User
                    {
                        Id = 2,
                        Email = "doctor1@gmail.com",
                        Money = 500m
                    }
                },
                new()
                {
                    Id = 3,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 2,
                    GenderType = GenderType.Female,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    SpecialtyId = 1,
                    User = new User
                    {
                        Id = 5,
                        Email = "doctor4@gmail.com",
                        Money = 400m
                    }
                }
            };

            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            var result = await _repository.GetDoctorAsync(id);

            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.FirstName.Should().Be("Глеб");
            result.LastName.Should().Be("Романенко");
            result.ExperienceYears.Should().Be(2);
            result.GenderType.Should().Be(GenderType.Male);
            result.WorkDayStart.Should().Be(new TimeSpan(9, 0, 0));
            result.WorkDayEnd.Should().Be(new TimeSpan(17, 0, 0));

            result.Specialty.Should().NotBeNull();
            result.Specialty.Id.Should().Be(1);
            result.Specialty.Name.Should().Be("Терапия");
            result.Specialty.Price.Should().Be(40);

            result.User.Should().NotBeNull();
            result.User.Id.Should().Be(2);
            result.User.Email.Should().Be("doctor1@gmail.com");
            result.User.Money.Should().Be(500m);
        }

        [Fact]
        public async Task GetDoctorByUserAsync_ShouldReturnDoctorByUserFromDb()
        {
            var userId = 2;

            var doctors = new List<Doctor>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    ExperienceYears = 4,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new Specialty
                    {
                        Id = 2,
                        Name = "Кардиология",
                        Price = 80
                    },
                    User = new User
                    {
                        Id = 1,
                        Email = "doctor24@gmail.com",
                        Money = 100m
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },
                    User = new User
                    {
                        Id = userId,
                        Email = "doctor1@gmail.com",
                        Money = 500m
                    }
                },
                new()
                {
                    Id = 3,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 2,
                    GenderType = GenderType.Female,
                    WorkDayStart = new TimeSpan(9, 0, 0),
                    WorkDayEnd = new TimeSpan(17, 0, 0),
                    SpecialtyId = 1,
                    User = new User
                    {
                        Id = 5,
                        Email = "doctor4@gmail.com",
                        Money = 400m
                    }
                }
            };

            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            var result = await _repository.GetDoctorByUserAsync(userId);

            result.Should().NotBeNull();
            result.Id.Should().Be(2);
            result.FirstName.Should().Be("Глеб");
            result.LastName.Should().Be("Романенко");
            result.ExperienceYears.Should().Be(2);
            result.GenderType.Should().Be(GenderType.Male);
            result.WorkDayStart.Should().Be(new TimeSpan(9, 0, 0));
            result.WorkDayEnd.Should().Be(new TimeSpan(17, 0, 0));

            result.Specialty.Should().NotBeNull();
            result.Specialty.Id.Should().Be(1);
            result.Specialty.Name.Should().Be("Терапия");
            result.Specialty.Price.Should().Be(40);

            result.User.Should().NotBeNull();
            result.User.Id.Should().Be(userId);
            result.User.Email.Should().Be("doctor1@gmail.com");
            result.User.Money.Should().Be(500m);
        }

        [Fact]
        public async Task CreateDoctorAsync_ShouldAddDoctorNotificationToDb()
        {
            var doctorToAdd = new Doctor
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                SpecialtyId = 2,
                Specialty = new Specialty
                {
                    Id = 2,
                    Name = "Кардиология",
                    Price = 80
                },
                User = new User
                {
                    Id = 2,
                    Email = "doctor24@gmail.com",
                    Money = 100m
                }
            };

            await _repository.CreateDoctorAsync(doctorToAdd);
            await _context.SaveChangesAsync();

            var doctor = await _context.Doctors.FirstOrDefaultAsync();

            doctor.Should().NotBeNull();
            doctor.FirstName.Should().Be("Foo");
            doctor.LastName.Should().Be("Too");
            doctor.ExperienceYears.Should().Be(4);
            doctor.GenderType.Should().Be(GenderType.Male);
            doctor.WorkDayStart.Should().Be(new TimeSpan(9, 0, 0));
            doctor.WorkDayEnd.Should().Be(new TimeSpan(17, 0, 0));
            
            doctor.Specialty.Should().NotBeNull();
            doctor.Specialty.Id.Should().Be(2);
            doctor.Specialty.Name.Should().Be("Кардиология");
            doctor.Specialty.Price.Should().Be(80);
            
            doctor.User.Should().NotBeNull();
            doctor.User.Id.Should().Be(2);
            doctor.User.Email.Should().Be("doctor24@gmail.com");
            doctor.User.Money.Should().Be(100m);
        }
    }
}
