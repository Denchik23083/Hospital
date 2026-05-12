using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Hospital.Controllers;
using Hospital.Services.DoctorService;
using Hospital.Services.SpecialtyService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class SpecialtyControllerTests
    {
        private readonly Mock<ISpecialtyService> _service;
        private readonly Mock<IDoctorService> _doctorService;
        private readonly SpecialtyController _controller;

        public SpecialtyControllerTests()
        {
            _service = new Mock<ISpecialtyService>();
            _doctorService = new Mock<IDoctorService>();
            _controller = new SpecialtyController(_service.Object, _doctorService.Object);
        }

        [Fact] 
        public async Task GetAllSpecialtiesAsync_ShouldReturnOk_WithListSpecialties()
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

            _service
                .Setup(_ => _.GetAllSpecialtiesAsync())
                .ReturnsAsync(specialties);

            var result = await _controller.GetAllSpecialtiesAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(specialties);

            _service.Verify(_ => _.GetAllSpecialtiesAsync(), Times.Once);
        }
    }
}
