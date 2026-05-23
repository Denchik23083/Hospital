using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Repositories.SpecialtyRepository;
using Hospital.Services.SpecialtyService;
using Moq;

namespace Hospital.Tests.Services
{
    public class SpecialtyServiceTests
    {
        private readonly Mock<ISpecialtyRepository> _repository;
        private readonly SpecialtyService _service;

        public SpecialtyServiceTests()
        {
            _repository = new Mock<ISpecialtyRepository>();
            _service = new SpecialtyService(_repository.Object);
        }

        //Method
        [Fact]
        public async Task GetAllSpecialtiesAsync_ShouldReturnListSpecialties()
        {
            var specialties = new List<SpecialtyResponse>
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

            _repository
                .Setup(_ => _.GetAllSpecialtiesAsync())
                .ReturnsAsync(specialties);

            var result = await _service.GetAllSpecialtiesAsync();

            result.Should().BeEquivalentTo(specialties);

            _repository.Verify(_ => _.GetAllSpecialtiesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSpecialtyPriceAsync_ShouldReturnDecimalPrice()
        {
            var specialtyId = 1;
            var price = 40m;

            _repository
                .Setup(_ => _.GetSpecialtyPriceAsync(specialtyId))
                .ReturnsAsync(price);

            var result = await _service.GetSpecialtyPriceAsync(specialtyId);

            result.Should().Be(price);

            _repository.Verify(_ => _.GetSpecialtyPriceAsync(specialtyId), Times.Once);
        }
    }
}
