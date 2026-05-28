using FluentAssertions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Hospital.Repositories.BookingRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

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

        [Fact]
        public async Task GetAllBookingsByDoctorAsync_ShouldReturnActiveBookingsByDoctorFromDb()
        {
            var doctorId = 2;

            var doctor = new Doctor
            {
                Id = doctorId,
                FirstName = "Глеб",
                LastName = "Романенко",
                Specialty = new Specialty 
                { 
                    Id = 1, 
                    Name = "Терапия", 
                    Price = 40m 
                },
                User = new User 
                { 
                    Id = 10, 
                    Email = "doctor10@gmail.com", 
                    Money = 100m 
                }
            };

            var patient = new Patient
            {
                Id = 1,
                User = new User 
                { 
                    Id = 20, 
                    Email = "patient@gmail.com", 
                    Money = 100m 
                }
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    PatientId = patient.Id,
                    Patient = patient,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot { Id = 1, DoctorId = doctorId, Doctor = doctor }
                },
                new()
                {
                    Id = 2,
                    PatientId = patient.Id,
                    Patient = patient,
                    BookingStatus = BookingStatus.Cancelled,
                    DoctorSlot = new DoctorSlot { Id = 2, DoctorId = doctorId, Doctor = doctor }
                },
                new()
                {
                    Id = 3,
                    PatientId = patient.Id,
                    Patient = patient,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot { Id = 3, DoctorId = 999 }
                }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllBookingsByDoctorAsync(doctorId);

            result.Should().ContainSingle();

            var booking = result.First();

            booking.Id.Should().Be(1);
            booking.PatientId.Should().Be(patient.Id);

            booking.Patient!.User!.Email.Should().Be("patient@gmail.com");

            booking.DoctorSlot!.Doctor!.FirstName.Should().Be("Глеб");
            booking.DoctorSlot.Doctor.Specialty!.Price.Should().Be(40m);
            booking.DoctorSlot.Doctor.User!.Email.Should().Be("doctor10@gmail.com");
        }

        [Fact]
        public async Task GetAllBookingsByPatientAsync_ShouldReturnActiveBookingsByPatientFromDb()
        {
            var patientId = 1;

            var doctor = new Doctor
            {
                Id = 2,
                FirstName = "Глеб",
                LastName = "Романенко",
                Specialty = new Specialty { Id = 1, Name = "Терапия", Price = 40m },
                User = new User { Id = 10, Email = "doctor@gmail.com", Money = 100m }
            };

            var patient = new Patient
            {
                Id = patientId,
                FirstName = "Foo",
                User = new User { Id = 20, Email = "patient@gmail.com", Money = 100m }
            };

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    PatientId = patientId,
                    Patient = patient,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot { Id = 1, DoctorId = doctor.Id, Doctor = doctor }
                },
                new()
                {
                    Id = 2,
                    PatientId = patientId,
                    Patient = patient,
                    BookingStatus = BookingStatus.Cancelled,
                    DoctorSlot = new DoctorSlot { Id = 2, DoctorId = doctor.Id, Doctor = doctor }
                },
                new()
                {
                    Id = 3,
                    PatientId = 999,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot { Id = 3, DoctorId = doctor.Id, Doctor = doctor }
                }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllBookingsByPatientAsync(patientId);

            result.Should().ContainSingle();

            var booking = result.First();

            booking.Id.Should().Be(1);
            booking.BookingStatus.Should().Be(BookingStatus.Active);

            booking.Patient!.FirstName.Should().Be("Foo");
            booking.Patient.User!.Email.Should().Be("patient@gmail.com");

            booking.DoctorSlot!.Doctor!.FirstName.Should().Be("Глеб");
            booking.DoctorSlot.Doctor.User!.Email.Should().Be("doctor@gmail.com");

            booking.DoctorSlot.Doctor.Specialty!.Name.Should().Be("Терапия");
            booking.DoctorSlot.Doctor.Specialty.Price.Should().Be(40m);
        }

        [Fact]
        public async Task GetBookingWithDoctorAsync_ShouldReturnBooking_WhenBookingBelongsToDoctor()
        {
            var bookingId = 1;
            var doctorId = 2;

            var booking = new Booking
            {
                Id = bookingId,
                PatientId = 1,
                BookingStatus = BookingStatus.Active,
                DoctorSlot = new DoctorSlot
                {
                    Id = 1,
                    DoctorId = doctorId
                }
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var result = await _repository.GetBookingWithDoctorAsync(bookingId, doctorId);

            result.Should().NotBeNull();

            result!.Id.Should().Be(bookingId);
            result.PatientId.Should().Be(1);
            result.BookingStatus.Should().Be(BookingStatus.Active);

            result.DoctorSlot!.Id.Should().Be(1);
            result.DoctorSlot.DoctorId.Should().Be(doctorId);
        }

        [Fact]
        public async Task GetBookingWithPatientAsync_ShouldReturnBooking_WhenBookingBelongsToPatient()
        {
            var bookingId = 1;
            var patientId = 1;

            var booking = new Booking
            {
                Id = bookingId,
                PatientId = patientId,
                BookingStatus = BookingStatus.Active,
                DoctorSlot = new DoctorSlot
                {
                    Id = 1,
                    DoctorId = 2,
                    Doctor = new Doctor
                    {
                        Id = 2,
                        FirstName = "Глеб",
                        LastName = "Романенко",
                        User = new User { Id = 10, Email = "doctor@gmail.com", Money = 100m },
                        Specialty = new Specialty { Id = 1, Name = "Терапия", Price = 40m }
                    }
                }
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var result = await _repository.GetBookingWithPatientAsync(bookingId, patientId);

            result.Should().NotBeNull();

            result!.Id.Should().Be(bookingId);
            result.PatientId.Should().Be(patientId);
            result.BookingStatus.Should().Be(BookingStatus.Active);

            result.DoctorSlot!.DoctorId.Should().Be(2);

            result.DoctorSlot.Doctor!.FirstName.Should().Be("Глеб");
            result.DoctorSlot.Doctor.LastName.Should().Be("Романенко");

            result.DoctorSlot.Doctor.User!.Email.Should().Be("doctor@gmail.com");

            result.DoctorSlot.Doctor.Specialty!.Name.Should().Be("Терапия");
            result.DoctorSlot.Doctor.Specialty.Price.Should().Be(40m);
        }

        [Fact]
        public async Task HasActiveBookingWithDoctorAsync_ShouldReturnTrue_WhenPatientHasActiveBookingWithDoctor()
        {
            var patientId = 1;
            var doctorId = 2;

            var booking = new Booking
            {
                Id = 1,
                PatientId = patientId,
                BookingStatus = BookingStatus.Active,
                DoctorSlot = new DoctorSlot
                {
                    Id = 1,
                    DoctorId = doctorId
                }
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var result = await _repository.HasActiveBookingWithDoctorAsync(patientId, doctorId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasActiveBookingWithDoctorAsync_ShouldReturnFalse_WhenPatientHasNoActiveBookingWithDoctor()
        {
            var patientId = 1;
            var doctorId = 2;

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1,
                    PatientId = patientId,
                    BookingStatus = BookingStatus.Cancelled,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 1,
                        DoctorId = doctorId
                    }
                },
                new()
                {
                    Id = 2,
                    PatientId = patientId,
                    BookingStatus = BookingStatus.Active,
                    DoctorSlot = new DoctorSlot
                    {
                        Id = 2,
                        DoctorId = 999
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
                        DoctorId = doctorId
                    }
                }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            var result = await _repository.HasActiveBookingWithDoctorAsync(patientId, doctorId);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddBookingAsync_ShouldAddBookingToDb()
        {
            var booking = new Booking
            {
                Id = 1,
                PatientId = 1,
                DoctorSlotId = 2,
                BookingStatus = BookingStatus.Active,
                CreatedAt = new DateTime(2026, 02, 03, 10, 00, 00)
            };

            await _repository.AddBookingAsync(booking);
            await _context.SaveChangesAsync();

            var result = await _context.Bookings.FirstOrDefaultAsync(_ => _.Id == booking.Id);

            result.Should().NotBeNull();

            result!.Id.Should().Be(1);
            result.PatientId.Should().Be(1);
            result.DoctorSlotId.Should().Be(2);
            result.BookingStatus.Should().Be(BookingStatus.Active);
            result.CreatedAt.Should().Be(new DateTime(2026, 02, 03, 10, 00, 00));
        }
    }
}
