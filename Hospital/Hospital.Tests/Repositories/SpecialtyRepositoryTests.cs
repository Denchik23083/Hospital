using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Repositories.SpecialtyRepository;
using Hospital.Tests.Helpers;
using Moq;

namespace Hospital.Tests.Repositories
{
    public class SpecialtyRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly SpecialtyRepository _repository;

        public SpecialtyRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new SpecialtyRepository(_context);
        }

        [Fact]
        public async Task GetAllSpecialtiesAsync_ShouldReturnListSpecialtiesFromDb()
        {
            var specialties = new List<Specialty>
            {
                new()
                {
                    Name = "Терапия",
                    Price = 40
                },
                new()
                {
                    Name = "Кардиология",
                    Price = 80
                },
                new()
                {
                    Name = "Неврология",
                    Price = 75
                },
                new()
                {
                    Name = "Офтальмология",
                    Price = 50
                },
                new()
                {
                    Name = "Ортопедия",
                    Price = 70
                },
                new()
                {
                    Name = "Эндокринология",
                    Price = 65
                },
                new()
                {
                    Name = "Пульмонология",
                    Price = 70
                },
                new()
                {
                    Name = "Психиатрия",
                    Price = 90
                },
                new()
                {
                    Name = "Стоматология",
                    Price = 85
                }
            };

            var specialtiesResponse = new List<SpecialtyResponse>
            {
                new()
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                new()
                {
                    Id = 2,
                    Name = "Кардиология",
                    Price = 80
                },
                new()
                {
                    Id = 3,
                    Name = "Неврология",
                    Price = 75
                },
                new()
                {
                    Id = 4,
                    Name = "Офтальмология",
                    Price = 50
                },
                new()
                {
                    Id = 5,
                    Name = "Ортопедия",
                    Price = 70
                },
                new()
                {
                    Id = 6,
                    Name = "Эндокринология",
                    Price = 65
                },
                new()
                {
                    Id = 7,
                    Name = "Пульмонология",
                    Price = 70
                },
                new()
                {
                    Id = 8,
                    Name = "Психиатрия",
                    Price = 90
                },
                new()
                {
                    Id = 9,
                    Name = "Стоматология",
                    Price = 85
                }
            };

            await _context.Specialties.AddRangeAsync(specialties);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllSpecialtiesAsync();

            result.Should().BeEquivalentTo(specialtiesResponse, 
                options => options.Excluding(_ => _.Id));
        }
    }
}
