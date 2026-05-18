using FluentAssertions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Utilities;
using Hospital.Hospital.Controllers;
using Hospital.Services.DoctorService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class DoctorControllerTests
    {
        private readonly Mock<IDoctorService> _service;
        private readonly DoctorController _controller;

        public DoctorControllerTests()
        {
            _service = new Mock<IDoctorService>();
            _controller = new DoctorController(_service.Object);
        }

        [Fact]
        public async Task GetAllDoctorsAsync_ShouldReturnOk_WithListDoctors()
        {
            var doctors = new List<DoctorWithUserResponse>
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

            _service
                .Setup(_ => _.GetAllDoctorsAsync())
                .ReturnsAsync(doctors);

            var result = await _controller.GetAllDoctorsAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctors);

            _service.Verify(_ => _.GetAllDoctorsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDoctorByUserAsync_ShouldReturnOk_WithDoctorByUser()
        {
            var userId = 2;

            var doctor = new DoctorWithUserResponse
            {
                Id = 1,
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
            };

            _service
                .Setup(_ => _.GetDoctorByUserAsync(userId))
                .ReturnsAsync(doctor);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetDoctorByUserAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctor);

            _service.Verify(_ => _.GetDoctorByUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorAsync_ShouldReturnCreated()
        {
            var doctorRequest = new DoctorFullRequest
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                SpecialtyId = 2,
                Email = "doctor24@gmail.com",
                Password = "1111"
            };

            _service
                .Setup(_ => _.CreateDoctorAsync(doctorRequest))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateDoctorAsync(doctorRequest);

            result.Should().BeOfType<CreatedResult>();

            _service.Verify(_ => _.CreateDoctorAsync(doctorRequest), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorByUserAsync_ShouldReturnNoContent()
        {
            var userId = 2;

            var doctorRequest = new DoctorRequest("Foo", "Too", GenderType.Female);

            _service
                .Setup(_ => _.UpdateDoctorByUserAsync(doctorRequest, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.UpdateDoctorByUserAsync(doctorRequest);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.UpdateDoctorByUserAsync(doctorRequest, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ShouldReturnNoContent()
        {
            var doctorId = 1;

            var doctorRequest = new DoctorFullRequest
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                SpecialtyId = 2,
                Email = "doctor24@gmail.com",
                Password = "1111"
            };

            _service
                .Setup(_ => _.UpdateDoctorAsync(doctorRequest, doctorId))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateDoctorAsync(doctorRequest, doctorId);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.UpdateDoctorAsync(doctorRequest, doctorId), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldReturnNoContent()
        {
            var doctorId = 1;

            _service
                .Setup(_ => _.DeleteDoctorAsync(doctorId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteDoctorAsync(doctorId);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.DeleteDoctorAsync(doctorId), Times.Once);
        }
    }
}
