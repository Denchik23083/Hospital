using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hospital.Core.Utilities
{
    public class SeedSystemUsersService(
        HospitalContext context,
        IConfiguration configuration,
        ILogger<SeedSystemUsersService> logger)
    {
        private readonly HospitalContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<SeedSystemUsersService> _logger = logger;

        public async Task SeedAsync()
        {
            await SeedAdminAsync();
            await SeedDoctorsAsync();
        }

        private async Task SeedAdminAsync()
        {
            var adminEmail = _configuration["SeedAdmin:Email"];
            var adminPassword = _configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                _logger.LogWarning("Seed admin skipped: email or password configuration is missing.");
                return;
            }

            var adminExists = await _context.Users.AnyAsync(u => u.RoleType == RoleType.Admin);
            if (adminExists)
            {
                _logger.LogInformation("Seed admin skipped: admin already exists.");
                return;
            }

            var userWithSameEmailExists = await _context.Users.AnyAsync(u => u.Email == adminEmail);
            if (userWithSameEmailExists)
            {
                _logger.LogWarning("Seed admin skipped: user with email {Email} already exists.", adminEmail);
                return;
            }

            var admin = new User
            {
                Email = adminEmail,
                RoleType = RoleType.Admin
            };

            var passwordHasher = new PasswordHasher<User>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);

            //Do not need AddAsync
            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin user {Email} created successfully.", adminEmail);
        }

        private async Task SeedDoctorsAsync()
        {
            var doctorsToSeed = new List<(string Email, string FirstName, 
                string LastName, int ExperienceYears, int SpecialtyId, 
                GenderType GenderType, TimeSpan WorkDayStart, TimeSpan WorkDayEnd)>
            {
                ("doctor1@gmail.com", "Глеб", "Романенко", 2, 1, GenderType.Male, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                ("doctor2@gmail.com", "Семен", "Лобанов", 3, 1, GenderType.Male, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                ("doctor3@gmail.com", "Борис", "Левин", 2, 1, GenderType.Male, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                ("doctor4@gmail.com", "Варвара", "Черноус", 1, 1, GenderType.Female, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                ("doctor5@gmail.com", "Мария", "Колисниченко", 3, 2, GenderType.Female, new TimeSpan(9, 0, 0), new TimeSpan(16, 0, 0)),
                ("doctor6@gmail.com", "Светлана", "Чернышова", 1, 2, GenderType.Female, new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0)),
                ("doctor7@gmail.com", "Вячеслав", "Селезнев", 5, 2, GenderType.Male, new TimeSpan(9, 0, 0), new TimeSpan(15, 30, 0)),
                ("doctor8@gmail.com", "Станислав", "Башницен", 7, 3, GenderType.Male, new TimeSpan(10, 0, 0), new TimeSpan(16, 0, 0)),
                ("doctor9@gmail.com", "Васелиса", "Шмид", 3, 3, GenderType.Female, new TimeSpan(10, 30, 0), new TimeSpan(16, 0, 0)),
                ("doctor10@gmail.com", "Дарья", "Зайченко", 4, 4, GenderType.Female, new TimeSpan(9, 30, 0), new TimeSpan(16, 30, 0)),
                ("doctor11@gmail.com", "Анатолий", "Войченко", 1, 4, GenderType.Male, new TimeSpan(10, 0, 0), new TimeSpan(16, 30, 0)),
                ("doctor12@gmail.com", "Евгений", "Шевчук", 5, 5, GenderType.Male, new TimeSpan(10, 0, 0), new TimeSpan(17, 0, 0)),
                ("doctor13@gmail.com", "Катерина", "Главко", 2, 5, GenderType.Female, new TimeSpan(10, 30, 0), new TimeSpan(17, 0, 0)),
                ("doctor14@gmail.com", "Елизавета", "Сидорчук", 3, 6, GenderType.Female, new TimeSpan(9, 30, 0), new TimeSpan(15, 30, 0)),
                ("doctor15@gmail.com", "Петр", "Иващенко", 8, 6, GenderType.Male, new TimeSpan(10, 0, 0), new TimeSpan(15, 30, 0)),
                ("doctor16@gmail.com", "Тарас", "Гайдар", 2, 7, GenderType.Male, new TimeSpan(10, 0, 0), new TimeSpan(16, 30, 0)),
                ("doctor17@gmail.com", "Анастасия", "Громова", 5, 7, GenderType.Female, new TimeSpan(10, 30, 0), new TimeSpan(16, 30, 0)),
                ("doctor18@gmail.com", "Вероника", "Борова", 4, 8, GenderType.Female, new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0)),
                ("doctor19@gmail.com", "Оксана", "Свиридова", 2, 9, GenderType.Female, new TimeSpan(9, 0, 0), new TimeSpan(15, 0, 0)),
                ("doctor20@gmail.com", "Полина", "Ушакова", 3, 9, GenderType.Female, new TimeSpan(9, 30, 0), new TimeSpan(15, 30, 0)),
                ("doctor21@gmail.com", "Денис", "Никифоров", 6, 9, GenderType.Male, new TimeSpan(10, 0, 0), new TimeSpan(16, 0, 0))
            };

            var passwordHasher = new PasswordHasher<User>();

            var doctorPassword = _configuration["SeedDoctor:Password"];

            if (string.IsNullOrWhiteSpace(doctorPassword))
            {
                _logger.LogWarning("Seed doctor skipped: password configuration is missing.");
                return;
            }

            var doctorExists = await _context.Users.AnyAsync(u => u.RoleType == RoleType.Doctor);
            if (doctorExists)
            {
                _logger.LogInformation("Seed doctor skipped: doctor already exists.");
                return;
            }

            var usersToAdd = new List<User>();

            foreach (var doctorData in doctorsToSeed)
            {
                var user = new User
                {
                    Email = doctorData.Email,
                    RoleType = RoleType.Doctor,
                    Doctor = new()
                    {
                        FirstName = doctorData.FirstName,
                        LastName = doctorData.LastName,
                        ExperienceYears = doctorData.ExperienceYears,
                        SpecialtyId = doctorData.SpecialtyId,
                        GenderType = doctorData.GenderType,
                        WorkDayStart = doctorData.WorkDayStart,
                        WorkDayEnd = doctorData.WorkDayEnd
                    }
                };

                user.PasswordHash = passwordHasher.HashPassword(user, doctorPassword);

                usersToAdd.Add(user);
            }

            if (usersToAdd.Count == 0)
            {
                _logger.LogInformation("Seed doctors skipped: all doctor accounts already exist.");
                return;
            }

            _context.Users.AddRange(usersToAdd);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seed doctors completed: {Count} doctor accounts created.", usersToAdd.Count);
        }
    }
}
