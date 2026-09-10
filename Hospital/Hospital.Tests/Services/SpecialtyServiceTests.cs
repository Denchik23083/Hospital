using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Repositories.SpecialtyRepository;
using Hospital.Services.SpecialtyService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class SpecialtyServiceTests
    {
        private readonly Mock<ISpecialtyRepository> _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly ILogger<SpecialtyService> _logger;
        private readonly SpecialtyService _service;

        public SpecialtyServiceTests()
        {
            _repository = new Mock<ISpecialtyRepository>();
            _mapper = new Mock<IMapper>();
            _logger = Mock.Of<ILogger<SpecialtyService>>();
            _service = new SpecialtyService(_repository.Object, _mapper.Object, _logger);
        }

        //Throw Exception Condition
        [Fact]
        public async Task GetSpecialtyPriceAsync_ShouldThrowSpecialtyNotFoundException_Logger()
        {
            var id = 1;

            _repository
                .Setup(_ => _.GetSpecialtyPriceAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((decimal?)null);

            var action = async () => await _service.GetSpecialtyPriceAsync(id, CancellationToken.None);

            await action.Should().ThrowAsync<SpecialtyNotFoundException>();

            _repository.Verify(_ => _.GetSpecialtyPriceAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        //Method
        [Fact]
        public async Task GetAllSpecialtiesAsync_ShouldReturnListSpecialties()
        {
            var specialties = new List<Specialty>
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

            _repository
                .Setup(_ => _.GetAllSpecialtiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(specialties);

            _mapper
                .Setup(_ => _.Map<IEnumerable<SpecialtyResponse>>(specialties))
                .Returns(specialtiesResponse);

            var result = await _service.GetAllSpecialtiesAsync(CancellationToken.None);

            result.Should().BeEquivalentTo(specialtiesResponse);

            _repository.Verify(_ => _.GetAllSpecialtiesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<SpecialtyResponse>>(specialties), Times.Once);
        }

        [Fact]
        public async Task GetSpecialtyPriceAsync_ShouldReturnDecimalPrice()
        {
            var specialtyId = 1;
            var price = 40m;

            _repository
                .Setup(_ => _.GetSpecialtyPriceAsync(specialtyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(price);

            var result = await _service.GetSpecialtyPriceAsync(specialtyId, CancellationToken.None);

            result.Should().Be(price);

            _repository.Verify(_ => _.GetSpecialtyPriceAsync(specialtyId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
