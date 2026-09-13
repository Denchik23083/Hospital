using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.DoctorSlotService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class DoctorSlotServiceTests
    {
        private readonly Mock<IDoctorSlotRepository> _repository;
        private readonly Mock<IPatientRepository> _patientRepository;
        private readonly Mock<IBookingRepository> _bookingRepository;
        private readonly Mock<IDoctorRepository> _doctorRepository;
        private readonly ILogger<DoctorSlotService> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly DoctorSlotService _service;

        public DoctorSlotServiceTests()
        {
            _repository = new Mock<IDoctorSlotRepository>();
            _patientRepository = new Mock<IPatientRepository>();
            _bookingRepository = new Mock<IBookingRepository>();
            _doctorRepository = new Mock<IDoctorRepository>();
            _logger = Mock.Of<ILogger<DoctorSlotService>>();
            _mapper = new Mock<IMapper>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();

            _service = new DoctorSlotService(_repository.Object,
                _patientRepository.Object, _bookingRepository.Object,
                _doctorRepository.Object, _logger, _mapper.Object, _unitOfWorkRepository.Object);
        }

        //Throw Exception Condition
        [Fact]
        public async Task GetAllDoctorSlotsDatesByDoctorAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 5;

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetAllDoctorSlotsDatesByDoctorAsync(userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            
            _repository.Verify(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimesByDoctorAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 5;
            var date = new DateOnly(2026, 02, 03);

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetAllDoctorSlotsTimesByDoctorAsync(date, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 6;
            var doctorId = 2;

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.GetAllDoctorSlotsDatesAsync(doctorId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Never);
            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 6;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetAllDoctorSlotsDatesAsync(doctorId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);

            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesAsync_ShouldReturnEmptyWhenHasActiveBooking()
        {
            var userId = 6;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId
            };

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            var empty = new List<DateOnly>();

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _bookingRepository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.GetAllDoctorSlotsDatesAsync(doctorId, userId, CancellationToken.None);

            result.Should().BeEquivalentTo(empty);

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimeByDateAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 6;
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Never);
            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimeByDateAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 6;
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);

            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimeByDateAsync_ShouldReturnEmptyWhenHasActiveBooking()
        {
            var userId = 6;
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId
            };

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = 5,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            var empty = new List<DateOnly>();

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _bookingRepository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, CancellationToken.None);

            result.Should().BeEquivalentTo(empty);

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAdminDoctorSlotsDatesAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var doctorId = 2;

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetAllAdminDoctorSlotsDatesAsync(doctorId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAdminDoctorSlotsTimeByDateAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddDoctorSlotsAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 4;
            var date = new DateOnly(2026, 02, 03);

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.AddDoctorSlotsAsync(date, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.DoctorSlotsAlreadyExistsAsync(It.IsAny<int>(), date, It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.AddDoctorSlotsAsync(It.IsAny<List<DoctorSlot>>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddDoctorSlotsAsync_ShouldThrowDoctorSlotAlreadyExistsException_Logger()
        {
            var userId = 4;
            var date = new DateOnly(2026, 02, 03);

            var doctor = new Doctor
            {
                Id = 2,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.DoctorSlotsAlreadyExistsAsync(doctor.Id, date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var action = async () => await _service.AddDoctorSlotsAsync(date, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorSlotAlreadyExistsException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.DoctorSlotsAlreadyExistsAsync(doctor.Id, date, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.AddDoctorSlotsAsync(It.IsAny<List<DoctorSlot>>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorSlotsAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 4;

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.DeleteDoctorSlotsAsync(userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllExpiredDoctorSlotsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.DeleteDoctorSlotsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorSlotsAsync_ShouldNotDeleteDoctorSlots_WhenExpiredDoctorSlotsNotExists()
        {
            var userId = 4;

            var doctor = new Doctor
            {
                Id = 2,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetAllExpiredDoctorSlotsAsync(doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            await _service.DeleteDoctorSlotsAsync(userId, CancellationToken.None);

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllExpiredDoctorSlotsAsync(doctor.Id, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.DeleteDoctorSlotsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        //Method
        [Fact]
        public async Task GetAllDoctorSlotsDatesByDoctorAsync_ShouldReturnListDates()
        {
            var userId = 5;

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            var doctor = new Doctor
            {
                Id = 1,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dates);

            var result = await _service.GetAllDoctorSlotsDatesByDoctorAsync(userId, CancellationToken.None);

            result.Should().BeEquivalentTo(dates);

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllDoctorSlotsDatesByDoctorAsync(doctor.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimesByDoctorAsync_ShouldReturnListDoctorSlotsBooking()
        {
            var userId = 5;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    DoctorId = 1,
                    Date = date,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings =
                    [
                        new()
                        {
                            Id = 1,
                            BookingStatus = BookingStatus.Active,
                            CreatedAt = DateTime.UtcNow,
                            Patient = new Patient
                            {
                                Id = 1,
                                FirstName = "Foo",
                                LastName = "Too",
                                BirthDate = new DateOnly(2003, 08, 03),
                                GenderType = GenderType.Male,
                                Phone = "49999999"
                            }
                        }
                    ]
                },
                new()
                {
                    Id = 2,
                    DoctorId = 1,
                    Date = date,
                    StartTime = new TimeSpan(09, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    Bookings = []
                }
            };

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

            var doctor = new Doctor
            {
                Id = 1,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetAllDoctorSlotsTimesByDoctorAsync(doctor.Id, date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlots);

            _mapper
                .Setup(_ => _.Map<IEnumerable<DoctorSlotBookingResponse>>(doctorSlots))
                .Returns(doctorSlotsBooking);

            var result = await _service.GetAllDoctorSlotsTimesByDoctorAsync(date, userId, CancellationToken.None);

            result.Should().BeEquivalentTo(doctorSlotsBooking);

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllDoctorSlotsTimesByDoctorAsync(doctor.Id, date, It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<DoctorSlotBookingResponse>>(doctorSlots), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesAsync_ShouldReturnListDates()
        {
            var userId = 6;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId
            };

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _bookingRepository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repository
                .Setup(_ => _.GetAllDoctorSlotsDatesAsync(doctor.Id, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dates);

            var result = await _service.GetAllDoctorSlotsDatesAsync(doctorId, userId, CancellationToken.None);

            result.Should().BeEquivalentTo(dates);

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(doctor.Id, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimeByDateAsync_ShouldReturnListDoctorSlots()
        {
            var userId = 6;
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId
            };

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            var doctorSlots = new List<DoctorSlot>
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

            var doctorSlotsResponse = new List<DoctorSlotResponse>
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

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _bookingRepository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repository
                .Setup(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlots);

            _mapper
                .Setup(_ => _.Map<IEnumerable<DoctorSlotResponse>>(doctorSlots))
                .Returns(doctorSlotsResponse);

            var result = await _service.GetAllDoctorSlotsTimeByDateAsync(doctorId, date, userId, CancellationToken.None);

            result.Should().BeEquivalentTo(doctorSlotsResponse);

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
            _bookingRepository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<DoctorSlotResponse>>(doctorSlots), Times.Once);
        }

        [Fact]
        public async Task GetAllAdminDoctorSlotsDatesAsync_ShouldReturnListDates()
        {
            var doctorId = 2;

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = 5,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetAllDoctorSlotsDatesAsync(doctor.Id, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dates);

            var result = await _service.GetAllAdminDoctorSlotsDatesAsync(doctorId, CancellationToken.None);

            result.Should().BeEquivalentTo(dates);

            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllDoctorSlotsDatesAsync(doctor.Id, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAdminDoctorSlotsTimeByDateAsync_ShouldReturnListDoctorSlotsBooking()
        {
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlotsResponse = new List<DoctorSlotResponse>
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

            var doctorSlots = new List<DoctorSlot>
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

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = 5,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlots);

            _mapper
                .Setup(_ => _.Map<IEnumerable<DoctorSlotResponse>>(doctorSlots))
                .Returns(doctorSlotsResponse);

            var result = await _service.GetAllAdminDoctorSlotsTimeByDateAsync(doctorId, date, CancellationToken.None);

            result.Should().BeEquivalentTo(doctorSlotsResponse);

            _doctorRepository.Verify(_ => _.GetDoctorAsync(doctorId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllDoctorSlotsTimeByDateAsync(doctor.Id, date, It.IsAny<DateOnly>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<DoctorSlotResponse>>(doctorSlots), Times.Once);
        }

        [Fact]
        public async Task AddDoctorSlotsAsync_ShouldAddListDoctorSlots()
        {
            var userId = 4;
            var date = new DateOnly(2026, 02, 03);

            TimeSpan breakStart = new(13, 00, 00);
            TimeSpan breakEnd = new(14, 00, 00);
            TimeSpan slot = new(00, 30, 00);

            var doctor = new Doctor
            {
                Id = 2,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            List<DoctorSlot>? doctorSlots = null;

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.DoctorSlotsAlreadyExistsAsync(doctor.Id, date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repository
                .Setup(_ => _.AddDoctorSlotsAsync(It.IsAny<List<DoctorSlot>>(), It.IsAny<CancellationToken>()))
                .Callback<List<DoctorSlot>, CancellationToken>((l, ct) => doctorSlots = l)
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.AddDoctorSlotsAsync(date, userId, CancellationToken.None);

            doctorSlots.Should().NotBeNull();
            doctorSlots.Should().HaveCount(14);

            doctorSlots.Should().OnlyContain(_ => _.EndTime - _.StartTime == slot);
            doctorSlots.Should().OnlyContain(_ => _.DoctorId == doctor.Id);
            doctorSlots.Should().OnlyContain(_ => _.Date == date);
            doctorSlots.Should().NotContain(_ => _.StartTime >= breakStart && _.StartTime < breakEnd);

            doctorSlots.First().StartTime.Should().Be(doctor.WorkDayStart);
            doctorSlots.First().EndTime.Should().Be(doctor.WorkDayStart + slot);

            doctorSlots.Last().StartTime.Should().Be(doctor.WorkDayEnd - slot);
            doctorSlots.Last().EndTime.Should().Be(doctor.WorkDayEnd);

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.DoctorSlotsAlreadyExistsAsync(doctor.Id, date, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.AddDoctorSlotsAsync(It.IsAny<List<DoctorSlot>>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctorSlotsAsync_ShouldDeleteDoctorSlots_WhenExpiredDoctorSlotsExists()
        {
            var userId = 4;

            var doctor = new Doctor
            {
                Id = 2,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = new User
                {
                    Id = userId,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            var expiredDoctorSlots = new List<int>
            {
                1, 2, 3, 4, 5, 6
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetAllExpiredDoctorSlotsAsync(doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredDoctorSlots);

            _repository
                .Setup(_ => _.DeleteDoctorSlotsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.DeleteDoctorSlotsAsync(userId, CancellationToken.None);

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllExpiredDoctorSlotsAsync(doctor.Id, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.DeleteDoctorSlotsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}