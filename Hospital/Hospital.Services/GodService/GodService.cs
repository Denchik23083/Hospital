using Hospital.Core.Exceptions;
using Hospital.Core.Models.Responce;
using Hospital.Db.Utilities;
using Hospital.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Hospital.Services.GodService
{
    public class GodService(HospitalContext context,
            IMapper mapper,
            ILogger<GodService> logger) : IGodService
    {
        private readonly HospitalContext _context = context;
        private readonly ILogger<GodService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserResponce>> GetAllAdminsAsync()
        {
            return await _context.Users
                .Where(_ => _.RoleType == RoleType.Admin)
                .Select(_ => new UserResponce
                {
                    Id = _.Id,
                    UserName = _.UserName,
                    Email = _.Email
                }).ToListAsync();
        }

        public async Task<UserResponce> GetAdminAsync(int adminId)
        {
            var admin = await _context.Users
                    .Where(_ => _.RoleType == RoleType.Admin)
                    .FirstOrDefaultAsync(_ => _.Id == adminId);

            if (admin is null)
            {
                _logger.LogWarning("Admin not found");
                throw new UserNotFoundException($"Admin with id: {adminId} not found");
            }

            return _mapper.Map<UserResponce>(admin);
        }

        public async Task MakeAdminAsync(int userId)
        {
            var userToAdmin = await _context.Users
                .Where(_ => _.RoleType == RoleType.User)
                .FirstOrDefaultAsync(_ => _.Id == userId);

            if (userToAdmin is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException($"User with id: {userId} not found");
            }

            userToAdmin.RoleType = RoleType.Admin;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User become admin");
        }

        public async Task MakeUserAsync(int adminId)
        {
            var adminToUser = await _context.Users
                .Where(_ => _.RoleType == RoleType.Admin)
                .FirstOrDefaultAsync(_ => _.Id == adminId);

            if (adminToUser is null)
            {
                _logger.LogWarning("Admin not found");
                throw new UserNotFoundException($"Admin with id: {adminId} not found");
            }

            adminToUser.RoleType = RoleType.User;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin become user");
        }
    }
}
