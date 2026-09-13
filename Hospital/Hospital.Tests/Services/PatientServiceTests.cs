using AutoMapper;
using FluentAssertions;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Response;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.AuthRepository;
using Hospital.Repositories.BookingRepository;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.PatientRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Services.PatientService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hospital.Tests.Services
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly ILogger<PatientService> _logger;
        private readonly Mock<IAuthRepository> _authRespository;
        private readonly Mock<IBookingRepository> _bookingRespository;
        private readonly Mock<INotificationRepository> _notificationRespository;
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkRepository;
        private readonly Mock<IDbContextTransaction> _transaction;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _repository = new Mock<IPatientRepository>();
            _mapper = new Mock<IMapper>();
            _logger = Mock.Of<ILogger<PatientService>>();
            _authRespository = new Mock<IAuthRepository>();
            _bookingRespository = new Mock<IBookingRepository>();
            _notificationRespository = new Mock<INotificationRepository>();
            _unitOfWorkRepository = new Mock<IUnitOfWorkRepository>();
            _transaction = new Mock<IDbContextTransaction>();

            _service = new PatientService(_repository.Object, _mapper.Object,
                _logger, _authRespository.Object, _bookingRespository.Object,
                _notificationRespository.Object, _unitOfWorkRepository.Object);
        }

        //Throw Exception Condition
        [Fact]
        public async Task GetPatientByUserAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 1;

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.GetPatientByUserAsync(userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<PatientWithUserResponse>(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 1;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "pedro@gmail.com", "1111");

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.UpdatePatientAsync(model, userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldThrowUserNotFoundException_Logger()
        {
            var userId = 1;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "pedro@gmail.com", "1111");

            var patientToUpdate = new Patient
            {
                Id = 2,
                FirstName = "Ivan",
                LastName = "Vanko",
                BirthDate = new DateOnly(1990, 02, 02),
                GenderType = GenderType.Male,
                Phone = "+49000000",
                User = null
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            var action = async () => await _service.UpdatePatientAsync(model, userId, CancellationToken.None);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldNotCheckEmailUnique_WhenEmailWasNotChanged()
        {
            var userId = 3;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "too@gmail.com", "1111");

            var patientToUpdate = new Patient
            {
                Id = 2,
                FirstName = "Ivan",
                LastName = "Vanko",
                BirthDate = new DateOnly(1990, 02, 02),
                GenderType = GenderType.Male,
                Phone = "+49000000",
                User = new User
                {
                    Id = userId,
                    Email = "too@gmail.com",
                    Money = 7000m,
                    PasswordHash = "old-password-hash"
                }
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.UpdatePatientAsync(model, userId, CancellationToken.None);

            patientToUpdate.User.Email.Should().Be(model.Email);

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var userId = 3;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "pedro@gmail.com", "1111");

            var patientToUpdate = new Patient
            {
                Id = 2,
                FirstName = "Ivan",
                LastName = "Vanko",
                BirthDate = new DateOnly(1990, 02, 02),
                GenderType = GenderType.Male,
                Phone = "+49000000",
                User = new User
                {
                    Id = userId,
                    Email = "too@gmail.com",
                    Money = 7000m,
                    PasswordHash = "old-password-hash"
                }
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            _authRespository
                .Setup(_ => _.IsEmailNotUniqueAsync(model.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var action = async () => await _service.UpdatePatientAsync(model, userId, CancellationToken.None);

            await action.Should().ThrowAsync<ConflictException>();

            patientToUpdate.User.Email.Should().Be("too@gmail.com");

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(model.Email, It.IsAny<CancellationToken>()), Times.Once);
            
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldNotUpdatePassword_WhenPasswordIsEmpty()
        {
            var userId = 3;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "too@gmail.com", "");

            var patientToUpdate = new Patient
            {
                Id = 2,
                FirstName = "Ivan",
                LastName = "Vanko",
                BirthDate = new DateOnly(1990, 02, 02),
                GenderType = GenderType.Male,
                Phone = "+49000000",
                User = new User
                {
                    Id = userId,
                    Email = "too@gmail.com",
                    Money = 7000m,
                    PasswordHash = "old-password-hash"
                }
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.UpdatePatientAsync(model, userId, CancellationToken.None);

            patientToUpdate.User.PasswordHash.Should().Be("old-password-hash");

            patientToUpdate.User.Email.Should().Be("too@gmail.com");

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReplenishBalanceAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var userId = 4;

            var model = new PatientReplenishBalanceRequest(500m);

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.ReplenishBalanceAsync(model, userId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ReplenishBalanceAsync_ShouldThrowUserNotFoundException_Logger()
        {
            var userId = 4;

            var model = new PatientReplenishBalanceRequest(500m);

            var patientToUpdate = new Patient
            {
                Id = 3,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = null
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            var action = async () => await _service.ReplenishBalanceAsync(model, userId, CancellationToken.None);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldThrowPatientNotFoundException_Logger()
        {
            var patientId = 4;

            _repository
                .Setup(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            var action = async () => await _service.DeletePatientAsync(patientId, CancellationToken.None);

            await action.Should().ThrowAsync<PatientNotFoundException>();

            _repository.Verify(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldThrowUserNotFoundException_Logger()
        {
            var patientId = 4;

            var patientToDelete = new Patient
            {
                Id = patientId,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = null
            };

            _repository
                .Setup(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToDelete);

            var action = async () => await _service.DeletePatientAsync(patientId, CancellationToken.None);

            await action.Should().ThrowAsync<UserNotFoundException>();

            _repository.Verify(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldThrowDoctorNotFoundException_Logger()
        {
            var patientId = 4;

            var patientToDelete = new Patient
            {
                Id = patientId,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = new User
                {
                    Id = 5,
                    Email = "boris@gmail.com",
                    Money = 9500m,
                }
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    BookingStatus = BookingStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    PatientId = patientId,
                    DoctorSlot = null
                }
            };

            _repository
                .Setup(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            var action = async () => await _service.DeletePatientAsync(patientId, CancellationToken.None);

            await action.Should().ThrowAsync<DoctorNotFoundException>();

            _repository.Verify(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.DeletePatientAsync(patientToDelete, It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldThrowInsufficientFundsException_Logger()
        {
            var patientId = 4;
            var price = 100m;

            var patientToDelete = new Patient
            {
                Id = patientId,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = new User
                {
                    Id = 5,
                    Email = "boris@gmail.com",
                    Money = 9500m,
                }
            };

            var doctorUser = new User
            {
                Id = 10,
                Money = 50m
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    PatientId = patientId,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot
                    {
                        Doctor = new Doctor
                        {
                            User = doctorUser,
                            Specialty = new Specialty
                            {
                                Price = price
                            }
                        }
                    }
                }
            };

            _repository
                .Setup(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            var action = async () => await _service.DeletePatientAsync(patientId, CancellationToken.None);

            await action.Should().ThrowAsync<InsufficientFundsException>();

            _repository.Verify(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(_ => _.DeletePatientAsync(patientToDelete, It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        //Method
        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnListPatients()
        {
            var patientsResponse = new List<PatientWithUserResponse>
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

            var patients = new List<Patient>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Denys",
                    LastName = "Stark",
                    BirthDate = new DateOnly(2000, 01, 01),
                    GenderType = GenderType.Male,
                    Phone = "+4977777777",
                    User = new User
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
                    User = new User
                    {
                        Email = "too@gmail.com",
                        Money = 7000m
                    }
                }
            };

            _repository
                .Setup(_ => _.GetAllPatientsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(patients);

            _mapper
                .Setup(_ => _.Map<IEnumerable<PatientWithUserResponse>>(patients))
                .Returns(patientsResponse);

            var result = await _service.GetAllPatientsAsync(CancellationToken.None);

            result.Should().BeEquivalentTo(patientsResponse);

            _repository.Verify(_ => _.GetAllPatientsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<IEnumerable<PatientWithUserResponse>>(patients), Times.Once);
        }

        [Fact]
        public async Task GetPatientByUserAsync_ShouldReturnPatientByUser()
        {
            var userId = 1;

            var patient = new Patient
            {
                Id = 1,
                FirstName = "Denys",
                LastName = "Stark",
                BirthDate = new DateOnly(2000, 01, 01),
                GenderType = GenderType.Male,
                Phone = "+4977777777",
                User = new User
                {
                    Id = 1,
                    Email = "foo@gmail.com",
                    Money = 10000m,
                    RoleType = RoleType.Patient,
                }
            };

            var mappedPatient = new PatientWithUserResponse
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
                    Money = 10000m,
                }
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            _mapper
                .Setup(_ => _.Map<PatientWithUserResponse>(patient))
                .Returns(mappedPatient);

            var result = await _service.GetPatientByUserAsync(userId, CancellationToken.None);

            result.Should().BeEquivalentTo(mappedPatient);

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(_ => _.Map<PatientWithUserResponse>(patient), Times.Once);
        }

        [Fact]
        public async Task GetPatientBalanceAsync_ShouldReturnDecimalPrice()
        {
            var userId = 1;
            var balance = 10000m;

            _repository
                .Setup(_ => _.GetPatientBalanceAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(balance);

            var result = await _service.GetPatientBalanceAsync(userId, CancellationToken.None);

            result.Should().Be(balance);

            _repository.Verify(_ => _.GetPatientBalanceAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePatientAsync_ShouldUpdatePatient_WhenPatientExists()
        {
            var userId = 3;

            var model = new PatientRequest("Pedro", "Bedrosovich",
                new DateOnly(1980, 05, 06), GenderType.Male,
                "+38077777777", "pedro@gmail.com", "1111");

            var patientToUpdate = new Patient
            {
                Id = 2,
                FirstName = "Ivan",
                LastName = "Vanko",
                BirthDate = new DateOnly(1990, 02, 02),
                GenderType = GenderType.Male,
                Phone = "+49000000",
                User = new User
                {
                    Id = userId,
                    Email = "too@gmail.com",
                    Money = 7000m,
                    PasswordHash = "old-password-hash"
                }
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            _authRespository
                .Setup(_ => _.IsEmailNotUniqueAsync(model.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.UpdatePatientAsync(model, userId, CancellationToken.None);

            patientToUpdate.FirstName.Should().Be(model.FirstName);
            patientToUpdate.LastName.Should().Be(model.LastName);
            patientToUpdate.BirthDate.Should().Be(model.BirthDate);
            patientToUpdate.GenderType.Should().Be(model.GenderType);
            patientToUpdate.Phone.Should().Be(model.Phone);
            patientToUpdate.User.Email.Should().Be(model.Email);

            patientToUpdate.User.PasswordHash.Should().NotBe("old-password-hash");

            var verifyResult = new PasswordHasher<User>()
                .VerifyHashedPassword(patientToUpdate.User, 
                patientToUpdate.User.PasswordHash, model.Password);

            verifyResult.Should().Be(PasswordVerificationResult.Success);

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _authRespository.Verify(_ => _.IsEmailNotUniqueAsync(model.Email, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReplenishBalanceAsync_ShouldReplenishBalance_WhenPatientExists()
        {
            var userId = 4;

            var model = new PatientReplenishBalanceRequest(500m);

            var patientToUpdate = new Patient
            {
                Id = 3,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = new User
                {
                    Id = userId,
                    Email = "boris@gmail.com",
                    Money = 9500m,
                }
            };

            _repository
                .Setup(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToUpdate);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.ReplenishBalanceAsync(model, userId, CancellationToken.None);

            patientToUpdate.User.Money.Should().Be(10000m);

            _repository.Verify(_ => _.GetPatientByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldDeletePatient_WhenPatientExistsAndHasNoActiveBookings()
        {
            var patientId = 4;

            var patientToDelete = new Patient
            {
                Id = patientId,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = new User
                {
                    Id = 5,
                    Email = "boris@gmail.com",
                    Money = 9500m,
                }
            };

            _repository
                .Setup(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _repository
                .Setup(_ => _.DeletePatientAsync(patientToDelete, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.DeletePatientAsync(patientId, CancellationToken.None);

            _repository.Verify(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.DeletePatientAsync(patientToDelete, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
            _notificationRespository.Verify(_ => _.AddNotificationAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),Times.Never);
        }

        [Fact]
        public async Task DeletePatientAsync_ShouldRefundPatientAndChargeDoctorAndAddNotification_WhenPatientHasActiveBooking()
        {
            var patientId = 4;
            var price = 100m;

            var patientToDelete = new Patient
            {
                Id = patientId,
                FirstName = "Boris",
                LastName = "Britva",
                BirthDate = new DateOnly(1999, 03, 10),
                GenderType = GenderType.Male,
                Phone = "+490550000",
                User = new User
                {
                    Id = 5,
                    Email = "boris@gmail.com",
                    Money = 9500m,
                }
            };

            var doctorUser = new User
            {
                Id = 10,
                Money = 500m
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    PatientId = patientId,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot
                    {
                        Doctor = new Doctor
                        {
                            User = doctorUser,
                            Specialty = new Specialty
                            {
                                Price = price
                            }
                        }
                    }
                }
            };

            _repository
                .Setup(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patientToDelete);

            _unitOfWorkRepository
                .Setup(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transaction.Object);

            _bookingRespository
                .Setup(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            _notificationRespository
                .Setup(_ => _.AddNotificationAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repository
                .Setup(_ => _.DeletePatientAsync(patientToDelete, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkRepository
                .Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.DeletePatientAsync(patientId, CancellationToken.None);

            patientToDelete.User.Money.Should().Be(9600m);
            doctorUser.Money.Should().Be(400m);

            _repository.Verify(_ => _.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _bookingRespository.Verify(_ => _.GetAllBookingsByPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(_ => _.DeletePatientAsync(patientToDelete, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkRepository.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationRespository.Verify(_ => _.AddNotificationAsync(
                It.Is<Notification>(notification =>
                    notification.UserId == doctorUser.Id &&
                    notification.Message.Contains("Boris") &&
                    notification.Message.Contains("Britva")),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}