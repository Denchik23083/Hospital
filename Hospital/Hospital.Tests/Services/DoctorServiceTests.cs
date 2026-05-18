using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.DoctorRepository;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.DoctorService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly ILogger<DoctorService> _logger;
        private readonly Mock<IAuthRepository> _authRespository;
        private readonly Mock<IBookingRepository> _bookingRespository;
        private readonly Mock<INotificationRepository> _notificationRespository;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly Mock<IDbContextTransaction> _transaction;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _repository = new Mock<IDoctorRepository>();
            _mapper = new Mock<IMapper>();
            _logger = Mock.Of<ILogger<DoctorService>>();
            _authRespository = new Mock<IAuthRepository>();
            _bookingRespository = new Mock<IBookingRepository>();
            _notificationRespository = new Mock<INotificationRepository>();
            _transaction = new Mock<IDbContextTransaction>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();

            _service = new DoctorService(_repository.Object, _mapper.Object,
                _logger, _authRespository.Object, _bookingRespository.Object,
                _notificationRespository.Object, _unitOfWorkRepository.Object);
        }

        //Throw Exception Condition
        [Fact]
        public async Task GetDoctorByUserAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 2;

            _repository
                .Setup(_ => _.GetDoctorByUserAsync(userId))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.GetDoctorByUserAsync(userId);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _repository.Verify(_ => _.GetDoctorByUserAsync(userId), Times.Once);
            _mapper.Verify(_ => _.Map<DoctorWithUserResponse>(It.IsAny<Doctor>()), Times.Never);
        }

        [Theory]
        [InlineData(9, 15, 17, 0)]
        [InlineData(9, 0, 16, 45)]
        [InlineData(9, 15, 16, 45)]
        public async Task CreateDoctorAsync_ShouldThrowDoctorWorkTimeException_Logger(
            int startHour,
            int startMinute,
            int endHour,
            int endMinute)
        {
            var doctorToAdd = new DoctorFullRequest
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(startHour, startMinute, 0),
                WorkDayEnd = new TimeSpan(endHour, endMinute, 0),
                SpecialtyId = 2,
                Email = "doctor24@gmail.com",
                Password = "1111"
            };

            var action = async () => await _service.CreateDoctorAsync(doctorToAdd);

            await action.Should().ThrowAsync<DoctorWorkTimeException>();

            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(It.IsAny<string>()), Times.Never);
            _mapper.Verify(_ => _.Map<Doctor>(It.IsAny<DoctorFullRequest>()), Times.Never);
            _repository.Verify(_ => _.CreateDoctorAsync(It.IsAny<Doctor>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateDoctorAsync_ShouldThrowConflictException_Logger()
        {
            var doctorToAdd = new DoctorFullRequest
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

            _authRespository
                .Setup(_ => _.IsEmailNotUniqueAsync(doctorToAdd.Email))
                .ReturnsAsync(true);

            var action = async () => await _service.CreateDoctorAsync(doctorToAdd);

            await action.Should().ThrowAsync<ConflictException>();

            _mapper.Verify(_ => _.Map<Doctor>(It.IsAny<DoctorFullRequest>()), Times.Never);
            _repository.Verify(_ => _.CreateDoctorAsync(It.IsAny<Doctor>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorByUserAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var userId = 2;

            var doctorRequest = new DoctorRequest("Foo", "Too", GenderType.Female);

            _repository
                .Setup(_ => _.GetDoctorByUserAsync(userId))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.UpdateDoctorByUserAsync(doctorRequest, userId);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _repository.Verify(_ => _.GetDoctorByUserAsync(userId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData(9, 15, 17, 0)]
        [InlineData(9, 0, 16, 45)]
        [InlineData(9, 15, 16, 45)]
        public async Task UpdateDoctorAsync_ShouldThrowDoctorWorkTimeException_Logger(
            int startHour,
            int startMinute,
            int endHour,
            int endMinute)
        {
            var doctorId = 1;

            var doctorRequest = new DoctorFullRequest
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(startHour, startMinute, 0),
                WorkDayEnd = new TimeSpan(endHour, endMinute, 0),
                SpecialtyId = 2,
                Email = "doctor24@gmail.com",
                Password = "1111"
            };

            var action = async () => await _service.UpdateDoctorAsync(doctorRequest, doctorId);

            await action.Should().ThrowAsync<DoctorWorkTimeException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Never);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(It.IsAny<string>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ShouldThrowDoctorNotFoundException_Logger()
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

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.UpdateDoctorAsync(doctorRequest, doctorId);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(doctorRequest.Email), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ShouldThrowUserNotFoundException_Logger()
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

            var doctorToUpdate = new Doctor
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
                User = null
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToUpdate);

            var action = async () => await _service.UpdateDoctorAsync(doctorRequest, doctorId);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(doctorRequest.Email), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
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

            var doctorToUpdate = new Doctor
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
                    Id = 2,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToUpdate);

            _authRespository
                .Setup(_ => _.IsEmailNotUniqueAsync(doctorRequest.Email))
                .ReturnsAsync(true);

            var action = async () => await _service.UpdateDoctorAsync(doctorRequest, doctorId);

            await action.Should().ThrowAsync<ConflictException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(doctorRequest.Email), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ShouldNotUpdatePassword_WhenPasswordIsEmpty()
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
                Email = "doctor1@gmail.com",
                Password = ""
            };

            var doctorToUpdate = new Doctor
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
                    Id = 2,
                    Email = "doctor1@gmail.com",
                    Money = 500m,
                    PasswordHash = "old-password-hash"
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToUpdate);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.UpdateDoctorAsync(doctorRequest, doctorId);

            doctorToUpdate.User.PasswordHash.Should().Be("old-password-hash");

            doctorToUpdate.User.Email.Should().Be("doctor1@gmail.com");

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var doctorId = 1;

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync((Doctor?)null);

            var action = async () => await _service.DeleteDoctorAsync(doctorId);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldThrowUserNotFoundException_Logger()
        {
            var doctorId = 1;

            var doctorToDelete = new Doctor
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = 40
                },
                User = null
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            var action = async () => await _service.DeleteDoctorAsync(doctorId);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldThrowSpecialtyNotFoundException_Logger()
        {
            var doctorId = 1;

            var doctorToDelete = new Doctor
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = null,
                User = new User
                {
                    Email = "doctor24@gmail.com",
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            var action = async () => await _service.DeleteDoctorAsync(doctorId);

            await action.Should().ThrowAsync<SpecialtyNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldThrowInsufficientFundsException_Logger()
        {
            var doctorId = 1;
            var price = 400m;

            var doctorToDelete = new Doctor
            {
                Id = doctorId,
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = price
                },
                User = new User
                {
                    Email = "doctor24@gmail.com",
                    Money = 300m
                },
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 1,
                        DoctorId = doctorId
                    },
                    Patient = new Patient
                    {
                        User = new User
                        {
                            Id = 20,
                            Money = 100m
                        }
                    }
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync())
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByDoctorAsync(doctorId))
                .ReturnsAsync(bookings);

            _repository
                .Setup(_ => _.DeleteDoctorAsync(doctorToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var action = async () => await _service.DeleteDoctorAsync(doctorId);

            await action.Should().ThrowAsync<InsufficientFundsException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByDoctorAsync(doctorId), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
            _repository.Verify(_ => _.DeleteDoctorAsync(doctorToDelete), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var doctorId = 1;
            var price = 200m;

            var doctorToDelete = new Doctor
            {
                Id = doctorId,
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = price
                },
                User = new User
                {
                    Email = "doctor24@gmail.com",
                    Money = 300m
                },
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 1,
                        DoctorId = doctorId
                    },
                    Patient = null
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync())
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByDoctorAsync(doctorId))
                .ReturnsAsync(bookings);

            _repository
                .Setup(_ => _.DeleteDoctorAsync(doctorToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var action = async () => await _service.DeleteDoctorAsync(doctorId);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByDoctorAsync(doctorId), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
            _repository.Verify(_ => _.DeleteDoctorAsync(doctorToDelete), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldThrowUserNotFoundException_WhenPatientUserIsNull_Logger()
        {
            var doctorId = 1;
            var price = 200m;

            var doctorToDelete = new Doctor
            {
                Id = doctorId,
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = price
                },
                User = new User
                {
                    Email = "doctor24@gmail.com",
                    Money = 300m
                },
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 1,
                        DoctorId = doctorId
                    },
                    Patient = new Patient
                    {
                        User = null
                    }
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync())
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByDoctorAsync(doctorId))
                .ReturnsAsync(bookings);

            _repository
                .Setup(_ => _.DeleteDoctorAsync(doctorToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var action = async () => await _service.DeleteDoctorAsync(doctorId);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByDoctorAsync(doctorId), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
            _repository.Verify(_ => _.DeleteDoctorAsync(doctorToDelete), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        //Tests
        [Fact]
        public async Task GetAllDoctorsAsync_ShouldReturnListDoctors()
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

            _repository
                .Setup(_ => _.GetAllDoctorsAsync())
                .ReturnsAsync(doctors);

            var result = await _service.GetAllDoctorsAsync();

            result.Should().BeEquivalentTo(doctors);

            _repository.Verify(_ => _.GetAllDoctorsAsync(), Times.Once);
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

        [Fact]
        public async Task GetDoctorByUserAsync_ShouldReturnDoctorByUser()
        {
            var userId = 2;

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

            var mappedDoctor = new DoctorWithUserResponse
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

            _repository
                .Setup(_ => _.GetDoctorByUserAsync(userId))
                .ReturnsAsync(doctor);

            _mapper
                .Setup(_ => _.Map<DoctorWithUserResponse>(doctor))
                .Returns(mappedDoctor);

            var result = await _service.GetDoctorByUserAsync(userId);

            result.Should().BeEquivalentTo(mappedDoctor);

            _repository.Verify(_ => _.GetDoctorByUserAsync(userId), Times.Once);
            _mapper.Verify(_ => _.Map<DoctorWithUserResponse>(doctor), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorAsync_ShouldCreateDoctor_WhenDoctorIsValid()
        {
            var doctorToAdd = new DoctorFullRequest
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

            var doctor = new Doctor
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Male,
                WorkDayStart = new TimeSpan(9, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                SpecialtyId = 2,
            };

            _authRespository
                .Setup(_ => _.IsEmailNotUniqueAsync(doctorToAdd.Email))
                .ReturnsAsync(false);

            _mapper
                .Setup(_ => _.Map<Doctor>(doctorToAdd))
                .Returns(doctor);

            _repository
                .Setup(_ => _.CreateDoctorAsync(doctor))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.CreateDoctorAsync(doctorToAdd);

            doctor.User.Should().NotBeNull();
            doctor.User.Email.Should().Be("doctor24@gmail.com");
            doctor.User.RoleType.Should().Be(RoleType.Doctor);

            var verifyResult = new PasswordHasher<User>()
                .VerifyHashedPassword(doctor.User,
                doctor.User.PasswordHash, doctorToAdd.Password);

            verifyResult.Should().Be(PasswordVerificationResult.Success);

            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(doctorToAdd.Email), Times.Once);
            _mapper.Verify(_ => _.Map<Doctor>(doctorToAdd), Times.Once);
            _repository.Verify(_ => _.CreateDoctorAsync(doctor), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorByUserAsync_ShouldUpdateDoctorByUser_WhenDoctorExists()
        {
            var userId = 2;

            var doctorRequest = new DoctorRequest("Foo", "Too", GenderType.Female);

            var doctorToUpdate = new Doctor
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

            _repository
                .Setup(_ => _.GetDoctorByUserAsync(userId))
                .ReturnsAsync(doctorToUpdate);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.UpdateDoctorByUserAsync(doctorRequest, userId);

            doctorToUpdate.FirstName.Should().Be(doctorRequest.FirstName);
            doctorToUpdate.LastName.Should().Be(doctorRequest.LastName);
            doctorToUpdate.GenderType.Should().Be(doctorRequest.GenderType);

            _repository.Verify(_ => _.GetDoctorByUserAsync(userId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ShouldUpdateDoctor_WhenDoctorExists()
        {
            var doctorId = 1;

            var doctorRequest = new DoctorFullRequest
            {
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
                GenderType = GenderType.Female,
                WorkDayStart = new TimeSpan(10, 0, 0),
                WorkDayEnd = new TimeSpan(17, 0, 0),
                SpecialtyId = 2,
                Email = "doctor24@gmail.com",
                Password = "1111"
            };

            var doctorToUpdate = new Doctor
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
                    Id = 2,
                    Email = "doctor1@gmail.com",
                    Money = 500m
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToUpdate);

            _authRespository
                .Setup(_ => _.IsEmailNotUniqueAsync(doctorRequest.Email))
                .ReturnsAsync(false);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.UpdateDoctorAsync(doctorRequest, doctorId);

            doctorToUpdate.FirstName.Should().Be(doctorRequest.FirstName);
            doctorToUpdate.LastName.Should().Be(doctorRequest.LastName);
            doctorToUpdate.GenderType.Should().Be(doctorRequest.GenderType);
            doctorToUpdate.ExperienceYears.Should().Be(doctorRequest.ExperienceYears);
            doctorToUpdate.WorkDayStart.Should().Be(doctorRequest.WorkDayStart);
            doctorToUpdate.WorkDayEnd.Should().Be(doctorRequest.WorkDayEnd);
            doctorToUpdate.SpecialtyId.Should().Be(doctorRequest.SpecialtyId);
            doctorToUpdate.User.Email.Should().Be(doctorRequest.Email);

            var verifyResult = new PasswordHasher<User>()
                .VerifyHashedPassword(doctorToUpdate.User,
                    doctorToUpdate.User.PasswordHash, doctorRequest.Password);

            verifyResult.Should().Be(PasswordVerificationResult.Success);

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(doctorRequest.Email), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldDeleteDoctor_WhenDoctorExistsAndHasNoActiveBookings()
        {
            var doctorId = 1;

            var doctorToDelete = new Doctor
            {
                Id = doctorId,
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 4,
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
                    Email = "doctor24@gmail.com",
                    Money = 700m
                },
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync())
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByDoctorAsync(doctorId))
                .ReturnsAsync([]);

            _repository
                .Setup(_ => _.DeleteDoctorAsync(doctorToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.DeleteDoctorAsync(doctorId);

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByDoctorAsync(doctorId), Times.Once);
            _repository.Verify(_ => _.DeleteDoctorAsync(doctorToDelete), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ShouldRefundPatientsAndChargeDoctorAndAddNotification_WhenDoctorHasBookings()
        {
            var doctorId = 1;
            var price = 100m;

            var doctorToDelete = new Doctor
            {
                Id = doctorId,
                FirstName = "Foo",
                LastName = "Too",
                Specialty = new Specialty
                {
                    Id = 1,
                    Name = "Терапия",
                    Price = price
                },
                User = new User
                {
                    Id = 10,
                    Email = "doctor24@gmail.com",
                    Money = 700m
                }
            };

            var patientUser = new User
            {
                Id = 20,
                Money = 300m
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 1,
                        DoctorId = doctorId
                    },
                    Patient = new Patient
                    {
                        Id = 5,
                        User = patientUser
                    }
                }
            };

            _repository
                .Setup(_ => _.GetDoctorAsync(doctorId))
                .ReturnsAsync(doctorToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync())
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByDoctorAsync(doctorId))
                .ReturnsAsync(bookings);

            _repository
                .Setup(_ => _.DeleteDoctorAsync(doctorToDelete))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.DeleteDoctorAsync(doctorId);

            doctorToDelete.User.Money.Should().Be(600m);
            patientUser.Money.Should().Be(400m);

            _repository.Verify(_ => _.GetDoctorAsync(doctorId), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByDoctorAsync(doctorId), Times.Once);
            _repository.Verify(_ => _.DeleteDoctorAsync(doctorToDelete), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(
                It.Is<Notification>(notification =>
                    notification.UserId == patientUser.Id &&
                    notification.Message.Contains("Foo") &&
                    notification.Message.Contains("Too"))),
                Times.Once);
            
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
