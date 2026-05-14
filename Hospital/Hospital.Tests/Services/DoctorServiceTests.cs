using AutoMapper;
using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.DoctorService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly ILogger<DoctorService> _logger;
        private readonly Mock<IBookingRepository> _bookingRespository;
        private readonly Mock<IAuthRepository> _authRespository;
        private readonly Mock<INotificationRepository> _notificationRespository;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _repository = new Mock<IDoctorRepository>();
            _mapper = new Mock<IMapper>();
            _logger = Mock.Of<ILogger<DoctorService>>();
            _bookingRespository = new Mock<IBookingRepository>();
            _authRespository = new Mock<IAuthRepository>();
            _notificationRespository = new Mock<INotificationRepository>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();

            _service = new DoctorService(_repository.Object, _mapper.Object, 
                _logger, _bookingRespository.Object, _authRespository.Object, 
                _notificationRespository.Object, _unitOfWorkRepository.Object);
        }

        [Fact]
        public async Task GetAllDoctorsBySpecialtyAsync_ShouldReturnDoctorsBySpecialty()
        {
            var specialtyId = 1;

            var doctors = new List<DoctorResponse>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    ExperienceYears = 4,
                    GenderType = GenderType.Male
                },
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

            _repository
                .Setup(_ => _.GetAllDoctorsBySpecialtyAsync(specialtyId))
                .ReturnsAsync(doctors);

            var result = await _service.GetAllDoctorsBySpecialtyAsync(specialtyId);

            result.Should().BeEquivalentTo(doctors);

            _repository.Verify(_ => _.GetAllDoctorsBySpecialtyAsync(specialtyId), Times.Once);
        }
    }
}
