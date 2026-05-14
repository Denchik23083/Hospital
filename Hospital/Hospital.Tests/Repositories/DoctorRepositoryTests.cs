using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.DoctorRepository;
using Hospital.Tests.Helpers;
using Moq;

namespace Hospital.Tests.Repositories
{
    public class DoctorRepositoryTests
    {
        private HospitalContext _context;
        private readonly DoctorRepository _repository;

        public DoctorRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new DoctorRepository(_context);
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

            var doctorsResponce = new List<DoctorResponse>
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

            result.Should().BeEquivalentTo(doctorsResponce);
        }
    }
}
