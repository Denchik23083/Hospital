using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.AdminService
{
    public class AdminService(HospitalContext context,
            IMapper mapper,
            ILogger<AdminService> logger) : IAdminService
    {
        private readonly HospitalContext _context = context;
        private readonly ILogger<AdminService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            return await _context.Users
                    .Where(_ => _.RoleType == RoleType.Doctor 
                            || _.RoleType == RoleType.Patient)
                    .Select(_ => new UserResponse
                    {
                        Id = _.Id,
                        Email = _.Email
                    }).ToListAsync();
        }

        public async Task<UserResponse> GetUserAsync(int userId)
        {
            var user = await _context.Users
                    .Where(_ => _.RoleType == RoleType.Doctor
                            || _.RoleType == RoleType.Patient)
                    .FirstOrDefaultAsync(_ => _.Id == userId);

            if (user is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException($"User with id: {userId} not found");
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var userToDelete = await _context.Users
                    .Where(_ => _.RoleType == RoleType.Doctor
                            || _.RoleType == RoleType.Patient)
                    .FirstOrDefaultAsync(_ => _.Id == userId);

            if (userToDelete is null)
            {
                _logger.LogWarning("User not found");
                throw new UserNotFoundException($"User with id: {userId} not found");
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User was deleted");
        }
    }
}
