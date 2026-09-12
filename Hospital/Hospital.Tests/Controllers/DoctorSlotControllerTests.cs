using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db.Utilities;
using Hospital.Hospital.Controllers;
using Hospital.Services.DoctorSlotService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class DoctorSlotControllerTests
    {
        private readonly Mock<IDoctorSlotService> _service;
        private readonly DoctorSlotController _controller;

        public DoctorSlotControllerTests()
        {
            _service = new Mock<IDoctorSlotService>();
            _controller = new DoctorSlotController(_service.Object);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesByDoctorAsync_ShouldReturnOk_WithListDates()
        {
            var userId = 5;

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            _service
                .Setup(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dates);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsDatesByDoctorAsync(CancellationToken.None);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(dates);

            _service.Verify(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimesByDoctorAsync_ShouldReturnOk_WithListDoctorSlotsBooking()
        {
            var userId = 5;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlotsBooking = new List<DoctorSlotBookingResponse>
            {
                new()
                {
                    Id = 1,
                    Date = date,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    LastBooking = new BookingPatientResponse
                    {
                        Id = 1,
                        BookingStatus = BookingStatus.Active.ToString(),
                        PatientResponse = new PatientResponse
                        {
                            Id = 1,
                            FirstName = "Foo",
                            LastName = "Too",
                            BirthDate = new DateOnly(2003, 08, 03),
                            GenderType = GenderType.Male,
                            Phone = "49999999"
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Date = date,
                    StartTime = new TimeSpan(09, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    LastBooking = null
                }
            };

            _service
                .Setup(_ => _.GetAllDoctorSlotsTimesByDoctorAsync(date, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlotsBooking);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsTimesByDoctorAsync(date, CancellationToken.None);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctorSlotsBooking);

            _service.Verify(_ => _.GetAllDoctorSlotsTimesByDoctorAsync(date, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesAsync_ShouldReturnOk_WithListDates()
        {
            var userId = 6;
            var doctorId = 2;

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            _service
                .Setup(_ => _.GetAllDoctorSlotsDatesAsync(doctorId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dates);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsDatesAsync(doctorId, CancellationToken.None);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(dates);

            _service.Verify(_ => _.GetAllDoctorSlotsDatesAsync(doctorId, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimeByDateAsync_ShouldReturnOk_WithListDoctorSlots()
        {
            var userId = 6;
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlotResponse>
            {
                new()
                {
                    Id = 1,
                    Date = date,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 2,
                    Date = date,
                    StartTime = new TimeSpan(09, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    DoctorId = doctorId
                }
            };

            _service
                .Setup(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlots);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, CancellationToken.None);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctorSlots);

            _service.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAdminDoctorSlotsDatesAsync_ShouldReturnOk_WithListDates()
        {
            var doctorId = 2;

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            _service
                .Setup(_ => _.GetAllAdminDoctorSlotsDatesAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dates);

            var result = await _controller.GetAllAdminDoctorSlotsDatesAsync(doctorId, CancellationToken.None);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(dates);

            _service.Verify(_ => _.GetAllAdminDoctorSlotsDatesAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAdminDoctorSlotsTimeByDateAsync_ShouldReturnOk_WithListDoctorSlots()
        {
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlotResponse>
            {
                new()
                {
                    Id = 1,
                    Date = date,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 2,
                    Date = date,
                    StartTime = new TimeSpan(09, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    DoctorId = doctorId
                }
            };

            _service
                .Setup(_ => _.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlots);

            var result = await _controller.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date, CancellationToken.None);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctorSlots);

            _service.Verify(_ => _.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddDoctorSlotsAsync_ShouldReturnCreated()
        {
            var userId = 4;
            var date = new DateOnly(2026, 02, 03);

            _service
                .Setup(_ => _.AddDoctorSlotsAsync(date, userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.AddDoctorSlotsAsync(date, CancellationToken.None);

            result.Should().BeOfType<CreatedResult>();

            _service.Verify(_ => _.AddDoctorSlotsAsync(date, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctorSlotsAsync_ShouldReturnNoContent()
        {
            var userId = 4;

            _service
                .Setup(_ => _.DeleteDoctorSlotsAsync(userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.DeleteDoctorSlotsAsync(CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.DeleteDoctorSlotsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}