using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.BookingRepository;
using Hospital.Tests.Helpers;

namespace Hospital.Tests.Repositories
{
    public class BookingRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly BookingRepository _repository;

        public BookingRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new BookingRepository(_context);
        }

        [Fact]
        public async Task GetAllPatientBookingsAsync_ShouldReturnListBookingsFromDb()
        {
            var patientId = 1;

            var doctor = new Doctor
            {
                Id = 1,
                FirstName = "Foo",
                LastName = "Too",
                ExperienceYears = 2,
                GenderType = GenderType.Male
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
                        Id = 1,
                        Date = new DateOnly(2026, 02, 03),
                        StartTime = new TimeSpan(9, 00, 00),
                        EndTime = new TimeSpan(9, 30, 00),
                        Doctor = doctor
                    }
                },
                new()
                {
                    Id = 2,
                    PatientId = patientId,
                    BookingStatus = BookingStatus.Cancelled,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 2,
                        Date = new DateOnly(2026, 02, 03),
                        StartTime = new TimeSpan(9, 30, 00),
                        EndTime = new TimeSpan(10, 00, 00),
                        Doctor = doctor
                    }
                },
                new()
                {
                    Id = 3,
                    PatientId = 999,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 3,
                        Date = new DateOnly(2026, 02, 04),
                        StartTime = new TimeSpan(10, 00, 00),
                        EndTime = new TimeSpan(10, 30, 00),
                        Doctor = doctor
                    }
                }
            };

            var bookingsResponce = new List<BookingResponse>
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

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllPatientBookingsAsync(patientId);

            result.Should().BeEquivalentTo(bookingsResponce);
        }
    }
}
