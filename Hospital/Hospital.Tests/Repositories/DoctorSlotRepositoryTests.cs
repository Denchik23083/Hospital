using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.DoctorSlotRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Repositories
{
    public class DoctorSlotRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly DoctorSlotRepository _repository;

        public DoctorSlotRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new DoctorSlotRepository(_context);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesByDoctorAsync_ShouldReturnListDatesFromDb()
        {
            var doctorId = 1;

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    Date = new DateOnly(2026, 02, 03),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00)
                },
                new()
                {
                    Id = 2,
                    Date = new DateOnly(2026, 02, 04),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00)
                },
                new()
                {
                    Id = 3,
                    Date = new DateOnly(2026, 02, 05),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00)
                },
                new()
                {
                    Id = 4,
                    Date = new DateOnly(2026, 02, 03),
                    DoctorId = 2,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00)
                },
                new()
                {
                    Id = 5,
                    Date = new DateOnly(2026, 02, 04),
                    DoctorId = 2,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00)
                },
                new()
                {
                    Id = 6,
                    Date = new DateOnly(2026, 02, 05),
                    DoctorId = 2,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00)
                },
            };

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllDoctorSlotsDatesByDoctorAsync(doctorId);

            result.Should().BeEquivalentTo(dates);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimesByDoctorAsync_ShouldReturnListDoctorSlotsBookingFromDb()
        {
            var doctorId = 1;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    DoctorId = doctorId,
                    Date = date,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings =
                    [
                        new Booking
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2026, 02, 01, 10, 00, 00),
                            BookingStatus = BookingStatus.Active,
                            Patient = new Patient
                            {
                                Id = 1,
                                FirstName = "Foo",
                                LastName = "Too",
                                BirthDate = new DateOnly(2003, 08, 03),
                                GenderType = GenderType.Male,
                                Phone = "49999999"
                            }
                        },
                        new Booking
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2026, 01, 01, 10, 00, 00),
                            BookingStatus = BookingStatus.Cancelled,
                            Patient = new Patient
                            {
                                Id = 2,
                                FirstName = "Old",
                                LastName = "Patient",
                                BirthDate = new DateOnly(2000, 01, 01),
                                GenderType = GenderType.Male,
                                Phone = "111111"
                            }
                        }
                    ]
                },
                new()
                {
                    Id = 2,
                    DoctorId = doctorId,
                    Date = date,
                    StartTime = new TimeSpan(09, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    Bookings = []
                },
                new()
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 04),
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 30, 00),
                },
                new()
                {
                    Id = 4,
                    DoctorId = 999,
                    Date = date,
                    StartTime = new TimeSpan(11, 00, 00),
                    EndTime = new TimeSpan(11, 30, 00),
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

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllDoctorSlotsTimesByDoctorAsync(doctorId, date);

            result.Should().BeEquivalentTo(doctorSlotsBooking);
        }

        [Fact]
        public async Task GetAllDoctorSlotsDatesAsync_ShouldReturnListDatesFromDb()
        {
            var doctorId = 2;
            var today = new DateOnly(2026, 02, 01);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    Date = new DateOnly(2026, 02, 03),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings = []
                },
                new()
                {
                    Id = 2,
                    Date = new DateOnly(2026, 02, 04),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings = []
                },
                new()
                {
                    Id = 3,
                    Date = new DateOnly(2026, 02, 05),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings = []
                },
                new()
                {
                    Id = 4,
                    Date = new DateOnly(2026, 02, 06),
                    DoctorId = doctorId,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings =
                    [
                        new Booking
                        {
                            Id = 1,
                            BookingStatus = BookingStatus.Active
                        }
                    ]
                },
                new()
                {
                    Id = 5,
                    Date = new DateOnly(2026, 02, 07),
                    DoctorId = 999,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings = []
                }
            };

            var dates = new List<DateOnly>
            {
                new (2026, 02, 03),
                new (2026, 02, 04),
                new (2026, 02, 05)
            };

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllDoctorSlotsDatesAsync(doctorId, today);

            result.Should().BeEquivalentTo(dates);
        }

        [Fact]
        public async Task GetAllDoctorSlotsTimeByDateAsync_ShouldReturnListDoctorSlotsFromDb()
        {
            var doctorId = 2;
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    DoctorId = doctorId,
                    Date = date,
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    Bookings =
                    [
                        new Booking
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2026, 02, 01, 10, 00, 00),
                            BookingStatus = BookingStatus.Cancelled,
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
                    DoctorId = doctorId,
                    Date = date,
                    StartTime = new TimeSpan(09, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    Bookings = []
                },
                new()
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = new DateOnly(2026, 02, 04),
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 30, 00),
                },
                new()
                {
                    Id = 4,
                    DoctorId = 999,
                    Date = date,
                    StartTime = new TimeSpan(11, 00, 00),
                    EndTime = new TimeSpan(11, 30, 00),
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

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllDoctorSlotsTimeByDateAsync(doctorId, date);

            result.Should().BeEquivalentTo(doctorSlotsResponse);
        }

        [Fact]
        public async Task GetAllExpiredDoctorSlotsAsync_ShouldReturnExpiredDoctorSlotsWithoutActiveBookings()
        {
            var doctorId = 2;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    DoctorId = doctorId,
                    Date = today.AddDays(-1),
                    Bookings = []
                },
                new()
                {
                    Id = 2,
                    DoctorId = doctorId,
                    Date = today.AddDays(-2),
                    Bookings =
                    [
                        new Booking
                        {
                            Id = 1,
                            BookingStatus = BookingStatus.Cancelled
                        }
                    ]
                },
                new()
                {
                    Id = 3,
                    DoctorId = doctorId,
                    Date = today.AddDays(-3),
                    Bookings =
                    [
                        new Booking
                        {
                            Id = 2,
                            BookingStatus = BookingStatus.Active
                        }
                    ]
                },
                new()
                {
                    Id = 4,
                    DoctorId = doctorId,
                    Date = today.AddDays(1),
                    Bookings = []
                },
                new()
                {
                    Id = 5,
                    DoctorId = 999,
                    Date = today.AddDays(-1),
                    Bookings = []
                }
            };

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllExpiredDoctorSlotsAsync(doctorId);

            result.Should().BeEquivalentTo([1, 2]);
        }

        [Fact]
        public async Task GetDoctorSlotAsync_ShouldReturnDoctorSlotWithDoctorBookingsAndSpecialty()
        {
            var slotId = 1;

            var doctorSlot = new DoctorSlot
            {
                Id = slotId,
                DoctorId = 2,
                Date = new DateOnly(2026, 02, 03),
                StartTime = new TimeSpan(09, 00, 00),
                EndTime = new TimeSpan(09, 30, 00),
                Doctor = new Doctor
                {
                    Id = 2,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    GenderType = GenderType.Male,

                    User = new User
                    {
                        Id = 5,
                        Email = "doctor@gmail.com"
                    },

                    Specialty = new Specialty
                    {
                        Id = 1,
                        Name = "Терапия",
                        Price = 40
                    }
                },
                Bookings =
                [
                    new Booking
                    {
                        Id = 1,
                        BookingStatus = BookingStatus.Active
                    }
                ]
            };

            await _context.DoctorSlots.AddAsync(doctorSlot);
            await _context.SaveChangesAsync();

            var result = await _repository.GetDoctorSlotAsync(slotId);

            result.Should().NotBeNull();

            result.Doctor.Should().NotBeNull();
            result.Doctor.User.Should().NotBeNull();
            result.Doctor.Specialty.Should().NotBeNull();

            result.Bookings.Should().HaveCount(1);

            result.Id.Should().Be(slotId);
            result.Doctor.FirstName.Should().Be("Глеб");
            result.Doctor.User.Email.Should().Be("doctor@gmail.com");
            result.Doctor.Specialty.Name.Should().Be("Терапия");
        }

        [Fact]
        public async Task DoctorSlotsAlreadyExistsAsync_ShouldReturnTrue_WhenDoctorSlotsAlreadyExists()
        {
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    Date = date,
                    StartTime = new TimeSpan(9, 00, 00),
                    EndTime = new TimeSpan(9, 30, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 2,
                    Date = new DateOnly(2026, 02, 02),
                    StartTime = new TimeSpan(9, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 3,
                    Date = date,
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 30, 00),
                    DoctorId = 1
                },
            };

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.DoctorSlotsAlreadyExistsAsync(doctorId, date);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DoctorSlotsAlreadyExistsAsync_ShouldReturnFalse_WhenDoctorSlotsNotExists()
        {
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    Date = new DateOnly(2026, 02, 02),
                    StartTime = new TimeSpan(9, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 2,
                    Date = date,
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 30, 00),
                    DoctorId = 1
                },
            };

            await _context.DoctorSlots.AddRangeAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _repository.DoctorSlotsAlreadyExistsAsync(doctorId, date);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddDoctorSlotsAsync_ShouldAddDoctorSlots()
        {
            var doctorId = 2;
            var date = new DateOnly(2026, 02, 03);

            var doctorSlots = new List<DoctorSlot>
            {
                new()
                {
                    Id = 1,
                    Date = date,
                    StartTime = new TimeSpan(9, 00, 00),
                    EndTime = new TimeSpan(9, 30, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 2,
                    Date = date,
                    StartTime = new TimeSpan(9, 30, 00),
                    EndTime = new TimeSpan(10, 00, 00),
                    DoctorId = doctorId
                },
                new()
                {
                    Id = 3,
                    Date = date,
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 30, 00),
                    DoctorId = doctorId
                },
            };

            await _repository.AddDoctorSlotsAsync(doctorSlots);
            await _context.SaveChangesAsync();

            var result = await _context.DoctorSlots.ToListAsync();

            result.Should().BeEquivalentTo(doctorSlots);
        }

        [Fact]
        public async Task DeleteDoctorSlotsAsync_ShouldExecuteWithoutException()
        {
            var expiredDoctorSlots = new List<int> { 1, 2, 3 };

            var action = async () => await _repository.DeleteDoctorSlotsAsync(expiredDoctorSlots);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
