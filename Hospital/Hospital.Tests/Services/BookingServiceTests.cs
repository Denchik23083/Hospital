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
using Hospital.Services.BookingService;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _repository;
        private readonly Mock<IPatientRepository> _patientRepository;
        private readonly Mock<IDoctorSlotRepository> _doctorSlotRepository;
        private readonly Mock<IDoctorRepository> _doctorRepository;
        private readonly ILogger<BookingService> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly Mock<IDbContextTransaction> _transaction;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _repository = new Mock<IBookingRepository>();
            _patientRepository = new Mock<IPatientRepository>();
            _doctorSlotRepository = new Mock<IDoctorSlotRepository>();
            _doctorRepository = new Mock<IDoctorRepository>();
            _logger = Mock.Of<ILogger<BookingService>>();
            _mapper = new Mock<IMapper>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();
            _transaction = new Mock<IDbContextTransaction>();

            _service = new BookingService(_repository.Object,
                _patientRepository.Object, _doctorSlotRepository.Object,
                _doctorRepository.Object, _logger, _mapper.Object, _unitOfWorkRepository.Object);
        }

        //Throw Exception Condition
        [Fact]
        public async Task GetAllPatientBookingsAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 4;

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.GetAllPatientBookingsAsync(userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetAllPatientBookingsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 4;
            var slotId = 2;

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowDoctorSlotNotFoundException_Logger()
        {
            var userId = 4;
            var slotId = 2;

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

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DoctorSlot?)null);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorSlotNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowSlotAlreadyBookedException_LoggerWhenActiveExists()
        {
            var userId = 4;
            var slotId = 2;

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

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),

                Doctor = new Doctor
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,

                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },

                    User = new User
                    {
                        Id = 10,
                        Email = "doctor@gmail.com",
                        Money = 100m
                    }
                },

                Bookings =
                [
                    new Booking
                    {
                        Id = 1,
                        PatientId = 999,
                        DoctorSlotId = slotId,
                        BookingStatus = BookingStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    }
                ]
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlot);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<SlotAlreadyBookedException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowSlotAlreadyBookedException_LoggerWhenPatientHasActiveBooking()
        {
            var userId = 4;
            var slotId = 2;

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

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),

                Doctor = new Doctor
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,

                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },

                    User = new User
                    {
                        Id = 10,
                        Email = "doctor@gmail.com",
                        Money = 100m
                    }
                },

                Bookings = []
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlot);

            _repository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<SlotAlreadyBookedException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowUserNotFoundException_Logger()
        {
            var userId = 4;
            var slotId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,
                User = null
            };

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),

                Doctor = new Doctor
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,

                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },

                    User = new User
                    {
                        Id = 10,
                        Email = "doctor@gmail.com",
                        Money = 100m
                    }
                },

                Bookings = []
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlot);

            _repository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 4;
            var slotId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,
                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 100m
                }
            };

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),
                Doctor = null,
                Bookings = []
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlot);

            _repository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowInsufficientFundsException_Logger()
        {
            var userId = 4;
            var slotId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,
                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 10m
                }
            };

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),
                Doctor = new Doctor
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,

                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },

                    User = new User
                    {
                        Id = 10,
                        Email = "doctor@gmail.com",
                        Money = 100m
                    }
                },
                Bookings = []
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlot);

            _repository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            var action = async () => await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            await action.Should().ThrowAsync<InsufficientFundsException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteBookingAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 4;
            var id = 7;

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.CompleteBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetBookingWithDoctorAsync(id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteBookingAsync_ShouldThrowBookingNotFoundException_Logger()
        {
            var userId = 4;
            var id = 7;
            var doctorId = 2;

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                User = new User
                {
                    Id = userId,
                    Email = "doctor@gmail.com",
                    Money = 140m
                },
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40m
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetBookingWithDoctorAsync(id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            var action = async () => await _service.CompleteBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<BookingNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithDoctorAsync(id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteBookingAsync_ShouldThrowBookingNotFoundException_LoggerWhenStatusNotActive()
        {
            var userId = 4;
            var id = 7;
            var doctorId = 2;

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                User = new User
                {
                    Id = userId,
                    Email = "doctor@gmail.com",
                    Money = 140m
                },
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40m
                }
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = 1,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Cancelled,
                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),
                    Doctor = doctor
                },
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    BirthDate = new DateOnly(2003, 08, 03),
                    GenderType = GenderType.Male,
                    Phone = "49999999",
                    User = new User
                    {
                        Id = 10,
                        Email = "patient@gmail.com",
                        Money = 60m
                    }
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetBookingWithDoctorAsync(id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            var action = async () => await _service.CompleteBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<BookingNotFoundException>();

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithDoctorAsync(id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 4;
            var id = 7;

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.CancelBookingAsync(id, userId, It.IsAny<CancellationToken>());

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);

            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrowBookingNotFoundException_Logger()
        {
            var userId = 4;
            var id = 7;
            var patientId = 1;

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,

                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 60m
                }
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _repository
                .Setup(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            var action = async () => await _service.CancelBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<BookingNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrowBookingNotFoundException_LoggerWhenStatusNotActive()
        {
            var userId = 4;
            var id = 7;
            var patientId = 1;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,

                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 60m
                }
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = patientId,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Completed,

                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),

                    Doctor = new Doctor
                    {
                        Id = doctorId,
                        FirstName = "Глеб",
                        LastName = "Романенко",
                        ExperienceYears = 2,
                        GenderType = GenderType.Male,

                        Specialty = new Specialty
                        {
                            Id = 1,
                            Name = "Терапия",
                            Price = 40m
                        },

                        User = new User
                        {
                            Id = 10,
                            Email = "doctor@gmail.com",
                            Money = 140m
                        }
                    }
                }
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _repository
                .Setup(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            var action = async () => await _service.CancelBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<BookingNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrowUserNotFoundException_Logger()
        {
            var userId = 4;
            var id = 7;
            var patientId = 1;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,
                User = null
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = patientId,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Active,

                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),

                    Doctor = new Doctor
                    {
                        Id = doctorId,
                        FirstName = "Глеб",
                        LastName = "Романенко",
                        ExperienceYears = 2,
                        GenderType = GenderType.Male,

                        Specialty = new Specialty
                        {
                            Id = 1,
                            Name = "Терапия",
                            Price = 40m
                        },

                        User = new User
                        {
                            Id = 10,
                            Email = "doctor@gmail.com",
                            Money = 140m
                        }
                    }
                }
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _repository
                .Setup(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            var action = async () => await _service.CancelBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 4;
            var id = 7;
            var patientId = 1;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,

                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 60m
                }
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = patientId,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Active,

                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),
                    Doctor = null
                }
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _repository
                .Setup(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            var action = async () => await _service.CancelBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrowInsufficientFundsException_Logger()
        {
            var userId = 4;
            var id = 7;
            var patientId = 1;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,

                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 60m
                }
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = patientId,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Active,

                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),

                    Doctor = new Doctor
                    {
                        Id = doctorId,
                        FirstName = "Глеб",
                        LastName = "Романенко",
                        ExperienceYears = 2,
                        GenderType = GenderType.Male,

                        Specialty = new Specialty
                        {
                            Id = 1,
                            Name = "Терапия",
                            Price = 240m
                        },

                        User = new User
                        {
                            Id = 10,
                            Email = "doctor@gmail.com",
                            Money = 140m
                        }
                    }
                }
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _repository
                .Setup(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            var action = async () => await _service.CancelBookingAsync(id, userId, CancellationToken.None);

            await action.Should().ThrowAsync<InsufficientFundsException>();

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        //Method
        [Fact]
        public async Task GetAllPatientBookingsAsync_ShouldReturnListBookings()
        {
            var userId = 4;

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 1,
                        Date = new DateOnly(2026, 02, 03),
                        StartTime = new TimeSpan(9, 00, 00),
                        EndTime = new TimeSpan(9, 30, 00),
                        Doctor = new Doctor
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
                    BookingStatus = BookingStatus.Cancelled,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 2,
                        Date = new DateOnly(2026, 02, 03),
                        StartTime = new TimeSpan(9, 30, 00),
                        EndTime = new TimeSpan(10, 00, 00),
                        Doctor = new Doctor
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

            var bookingsResponse = new List<BookingResponse>
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

            _repository
                .Setup(_ => _.GetAllPatientBookingsAsync(patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            _mapper
                .Setup(_ => _.Map<IEnumerable<BookingResponse>>(bookings))
                .Returns(bookingsResponse);

            var result = await _service.GetAllPatientBookingsAsync(userId, CancellationToken.None);

            result.Should().BeEquivalentTo(bookingsResponse);

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetAllPatientBookingsAsync(patient.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<BookingResponse>>(bookings), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateBooking()
        {
            var userId = 4;
            var slotId = 2;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,
                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 100m
                }
            };

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),
                Doctor = new Doctor
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,

                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    },

                    User = new User
                    {
                        Id = 10,
                        Email = "doctor@gmail.com",
                        Money = 100m
                    }
                },
                Bookings = []
            };

            Booking? booking = null;

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _doctorSlotRepository
                .Setup(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctorSlot);

            _repository
                .Setup(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _repository
                .Setup(_ => _.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .Callback<Booking, CancellationToken>((b, ct) => booking = b)
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.CreateBookingAsync(slotId, userId, CancellationToken.None);

            patient.User.Money.Should().Be(60m);
            doctorSlot.Doctor.User.Money.Should().Be(140m);

            booking.Should().NotBeNull();

            booking.PatientId.Should().Be(patient.Id);
            booking.DoctorSlotId.Should().Be(doctorSlot.Id);
            booking.BookingStatus.Should().Be(BookingStatus.Active);
            booking.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _doctorSlotRepository.Verify(_ => _.GetDoctorSlotAsync(slotId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.HasActiveBookingWithDoctorAsync(patient.Id, doctorSlot.DoctorId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteBookingAsync_ShouldCompleteBooking()
        {
            var userId = 4;
            var id = 7;
            var doctorId = 2;

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                ExperienceYears = 2,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                User = new User
                {
                    Id = userId,
                    Email = "doctor@gmail.com",
                    Money = 140m
                },
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40m
                }
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = 1,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Active,
                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),
                    Doctor = doctor
                },
                Patient = new Patient
                {
                    Id = 1,
                    FirstName = "Foo",
                    LastName = "Too",
                    BirthDate = new DateOnly(2003, 08, 03),
                    GenderType = GenderType.Male,
                    Phone = "49999999",
                    User = new User
                    {
                        Id = 10,
                        Email = "patient@gmail.com",
                        Money = 60m
                    }
                }
            };

            _doctorRepository
                .Setup(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            _repository
                .Setup(_ => _.GetBookingWithDoctorAsync(id, doctor.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.CompleteBookingAsync(id, userId, CancellationToken.None);

            booking.BookingStatus.Should().Be(BookingStatus.Completed);

            _doctorRepository.Verify(_ => _.GetDoctorByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithDoctorAsync(id, doctor.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldCancelBooking()
        {
            var userId = 4;
            var id = 7;
            var patientId = 1;
            var doctorId = 2;

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                LastName = "Too",
                BirthDate = new DateOnly(2003, 08, 03),
                GenderType = GenderType.Male,
                Phone = "49999999",
                UserId = userId,

                User = new User
                {
                    Id = userId,
                    Email = "patient@gmail.com",
                    Money = 60m
                }
            };

            var booking = new Booking
            {
                Id = id,
                PatientId = patientId,
                DoctorSlotId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                BookingStatus = BookingStatus.Active,

                DoctorSlot = new DoctorSlot
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 03),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0),

                    Doctor = new Doctor
                    {
                        Id = doctorId,
                        FirstName = "Глеб",
                        LastName = "Романенко",
                        ExperienceYears = 2,
                        GenderType = GenderType.Male,

                        Specialty = new Specialty
                        {
                            Id = 1,
                            Name = "Терапия",
                            Price = 40m
                        },

                        User = new User
                        {
                            Id = 10,
                            Email = "doctor@gmail.com",
                            Money = 140m
                        }
                    }
                }
            };

            _patientRepository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _repository
                .Setup(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.CancelBookingAsync(id, userId, CancellationToken.None);

            patient.User.Money.Should().Be(100m);
            booking.DoctorSlot.Doctor.User.Money.Should().Be(100m);

            _patientRepository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.GetBookingWithPatientAsync(id, patient.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}