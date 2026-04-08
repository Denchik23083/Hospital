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
            await SeedGodAsync();
            await SeedAdminAsync();
            await SeedDoctorsAsync();
        }

        private async Task SeedGodAsync()
        {
            var godEmail = _configuration["SeedGod:Email"];
            var godPassword = _configuration["SeedGod:Password"];

            if (string.IsNullOrWhiteSpace(godEmail) || string.IsNullOrWhiteSpace(godPassword))
            {
                _logger.LogWarning("Seed god skipped: email or password configuration is missing.");
                return;
            }

            var godExists = await _context.Users.AnyAsync(u => u.RoleType == RoleType.God);
            if (godExists)
            {
                _logger.LogInformation("Seed god skipped: god already exists.");
                return;
            }

            var userWithSameEmailExists = await _context.Users.AnyAsync(u => u.Email == godEmail);
            if (userWithSameEmailExists)
            {
                _logger.LogWarning("Seed god skipped: user with email {Email} already exists.", godEmail);
                return;
            }

            var god = new User
            {
                Email = godEmail,
                RoleType = RoleType.God
            };

            var passwordHasher = new PasswordHasher<User>();
            god.PasswordHash = passwordHasher.HashPassword(god, godPassword);

            //Do not need AddAsync
            _context.Users.Add(god);
            await _context.SaveChangesAsync();

            _logger.LogInformation("God user {Email} created successfully.", godEmail);
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
            var doctorsToSeed = new List<(string Email, string FirstName, string LastName, int ExperienceYears, int SpecialtyId, GenderType GenderType)>
{
                ("doctor1@gmail.com", "Глеб", "Романенко", 2, 1, GenderType.Male),
                ("doctor2@gmail.com", "Семен", "Лобанов", 3, 1, GenderType.Male),
                ("doctor3@gmail.com", "Борис", "Левин", 2, 1, GenderType.Male),
                ("doctor4@gmail.com", "Варвара", "Черноус", 1, 1, GenderType.Female),
                ("doctor5@gmail.com", "Мария", "Колисниченко", 3, 2, GenderType.Female),
                ("doctor6@gmail.com", "Светлана", "Чернышова", 1, 2, GenderType.Female),
                ("doctor7@gmail.com", "Вячеслав", "Селезнев", 5, 2, GenderType.Male),
                ("doctor8@gmail.com", "Станислав", "Башницен", 7, 3, GenderType.Male),
                ("doctor9@gmail.com", "Васелиса", "Шмид", 3, 3, GenderType.Female),
                ("doctor10@gmail.com", "Дарья", "Зайченко", 4, 4, GenderType.Female),
                ("doctor11@gmail.com", "Анатолий", "Войченко", 1, 4, GenderType.Male),
                ("doctor12@gmail.com", "Евгений", "Шевчук", 5, 5, GenderType.Male),
                ("doctor13@gmail.com", "Катерина", "Главко", 2, 5, GenderType.Female),
                ("doctor14@gmail.com", "Елизавета", "Сидорчук", 3, 6, GenderType.Female),
                ("doctor15@gmail.com", "Петр", "Иващенко", 8, 6, GenderType.Male),
                ("doctor16@gmail.com", "Тарас", "Гайдар", 2, 7, GenderType.Male),
                ("doctor17@gmail.com", "Анастасия", "Громова", 5, 7, GenderType.Female),
                ("doctor18@gmail.com", "Вероника", "Борова", 4, 8, GenderType.Female),
                ("doctor19@gmail.com", "Оксана", "Свиридова", 2, 9, GenderType.Female),
                ("doctor20@gmail.com", "Полина", "Ушакова", 3, 9, GenderType.Female),
                ("doctor21@gmail.com", "Денис", "Никифоров", 6, 9, GenderType.Male)
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
                _logger.LogInformation("Seed doctor skipped: god already exists.");
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
                        GenderType = doctorData.GenderType
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
