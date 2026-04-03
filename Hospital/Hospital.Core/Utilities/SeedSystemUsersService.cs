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
        }

        private async Task SeedGodAsync()
        {
            var godUserName = _configuration["SeedGod:UserName"] ?? "god";
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
                UserName = godUserName,
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
            var adminUserName = _configuration["SeedAdmin:UserName"] ?? "admin";
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
                UserName = adminUserName,
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
    }
}