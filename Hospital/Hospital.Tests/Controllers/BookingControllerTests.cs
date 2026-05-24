using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db.Utilities;
using Hospital.Hospital.Controllers;
using Hospital.Services.BookingService;
using Hospital.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hospital.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<IBookingService> _service;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            _service = new Mock<IBookingService>();
            _controller = new BookingController(_service.Object);
        }

        [Fact]
        public async Task GetAllPatientBookingsAsync_ShouldReturnOk_WithListBookings()
        {
            var userId = 4;
            var bookings = new List<BookingResponse>
            {
                new()
                {
                    Id = 1,
                    BookingStatus = BookingStatus.Active.ToString(),
                    DoctorSlotWithDoctorResponse = new DoctorSlotWithDoctorResponse
                    {
                        Id = 1,
                        Date = new DateOnly(2026, 02, 03),
                        StartTime = new TimeSpan(9, 00, 00),
                        EndTime = new TimeSpan(9, 30, 00),
                        DoctorResponse = new DoctorResponse
                        {
                            Id = 1,
                            FirstName = "Foo",
                            LastName = "Too",
                            ExperienceYears = 2,
                            GenderType = GenderType.Male,
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    BookingStatus = BookingStatus.Cancelled.ToString(),
                    DoctorSlotWithDoctorResponse = new DoctorSlotWithDoctorResponse
                    {
                        Id = 2,
                        Date = new DateOnly(2026, 02, 03),
                        StartTime = new TimeSpan(9, 30, 00),
                        EndTime = new TimeSpan(10, 00, 00),
                        DoctorResponse = new DoctorResponse
                        {
                            Id = 1,
                            FirstName = "Foo",
                            LastName = "Too",
                            ExperienceYears = 2,
                            GenderType = GenderType.Male,
                        }
                    }
                }
            };

            _service
                .Setup(_ => _.GetAllPatientBookingsAsync(userId))
                .ReturnsAsync(bookings);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.GetAllPatientBookingsAsync();

            var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            actionResult.Value.Should().BeEquivalentTo(bookings);

            _service.Verify(_ => _.GetAllPatientBookingsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldReturnCreated()
        {
            var slotId = 3;
            var userId = 4;

            _service
                .Setup(_ => _.CreateBookingAsync(slotId, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.CreateBookingAsync(slotId);

            result.Should().BeOfType<CreatedResult>();

            _service.Verify(_ => _.CreateBookingAsync(slotId, userId), Times.Once);
        }

        [Fact]
        public async Task CompleteBookingAsync_ShouldReturnNoContent()
        {
            var id = 5;
            var userId = 4;

            _service
                .Setup(_ => _.CompleteBookingAsync(id, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.CompleteBookingAsync(id);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.CompleteBookingAsync(id, userId), Times.Once);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldReturnNoContent()
        {
            var id = 5;
            var userId = 4;

            _service
                .Setup(_ => _.CancelBookingAsync(id, userId))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = TestUserFactory.CreateControllerContext(userId);

            var result = await _controller.CancelBookingAsync(id);

            result.Should().BeOfType<NoContentResult>();

            _service.Verify(_ => _.CancelBookingAsync(id, userId), Times.Once);
        }
    }
}