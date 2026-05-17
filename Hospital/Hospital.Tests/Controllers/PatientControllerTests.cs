using FluentAssertions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Utilities;
using Hospital.Hospital.Controllers;
using Hospital.Services.PatientService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class PatientControllerTests
    {
        private readonly Mock<IPatientService> _service;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _service = new Mock<IPatientService>();
            _controller = new PatientController(_service.Object);
        }

        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnOk_WithListPatients()
        {
            var patients = new List<PatientWithUserResponse>
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

            _service
                .Setup(_ => _.GetAllPatientsAsync())
                .ReturnsAsync(patients);

            var result = await _controller.GetAllPatientsAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(patients);

            _service.Verify(_ => _.GetAllPatientsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPatientByUserAsync_ShouldReturnOk_WithPatientByUser()
        {
            var userId = 1;

            var patientWithUser = new PatientWithUserResponse
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
            };

            _service
                .Setup(_ => _.GetPatientByUserAsync(userId))
                .ReturnsAsync(patientWithUser);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetPatientByUserAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(patientWithUser);

            _service.Verify(_ => _.GetPatientByUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetPatientBalanceAsync_ShouldReturnOk_WithDecimalPrice()
        {
            var userId = 1;
            var balance = 10000m;

            _service
                .Setup(_ => _.GetPatientBalanceAsync(userId))
                .ReturnsAsync(balance);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetPatientBalanceAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().Be(balance);

            _service.Verify(_ => _.GetPatientBalanceAsync(userId), Times.Once);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldReturnNoContent()
        {
            var userId = 3;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "pedro@gmail.com", "1111");

            _service
                .Setup(_ => _.UpdatePatientAsync(model, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.UpdatePatientAsync(model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ReplenishBalanceAsync_ShouldReturnNoContent()
        {
            var userId = 4;

            var model = new PatientReplenishBalanceRequest(500m);

            _service
                .Setup(_ => _.ReplenishBalanceAsync(model, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.ReplenishBalanceAsync(model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldReturnNoContent()
        {
            var patientId = 4;

            _service
                .Setup(_ => _.DeletePatientAsync(patientId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeletePatientAsync(patientId);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
