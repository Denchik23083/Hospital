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
                .Setup(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(userId))
                .ReturnsAsync(dates);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsDatesByDoctorAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(dates);

            _service.Verify(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(userId), Times.Once);
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
                .Setup(_ => _.GetAllDoctorSlotsTimesByDoctorAsync(date, userId))
                .ReturnsAsync(doctorSlotsBooking);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsTimesByDoctorAsync(date);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctorSlotsBooking);

            _service.Verify(_ => _.GetAllDoctorSlotsTimesByDoctorAsync(date, userId), Times.Once);
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
                .Setup(_ => _.GetAllDoctorSlotsDatesAsync(doctorId, userId))
                .ReturnsAsync(dates);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsDatesAsync(doctorId);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(dates);

            _service.Verify(_ => _.GetAllDoctorSlotsDatesAsync(doctorId, userId), Times.Once);
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
                .Setup(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId))
                .ReturnsAsync(doctorSlots);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllDoctorSlotsTimeByDateAsync(doctorId, date);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctorSlots);

            _service.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId), Times.Once);
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
                .Setup(_ => _.GetAllAdminDoctorSlotsDatesAsync(doctorId))
                .ReturnsAsync(dates);

            var result = await _controller.GetAllAdminDoctorSlotsDatesAsync(doctorId);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(dates);

            _service.Verify(_ => _.GetAllAdminDoctorSlotsDatesAsync(doctorId), Times.Once);
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
                .Setup(_ => _.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date))
                .ReturnsAsync(doctorSlots);

            var result = await _controller.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date);

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(doctorSlots);

            _service.Verify(_ => _.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date), Times.Once);
        }
    }
}
