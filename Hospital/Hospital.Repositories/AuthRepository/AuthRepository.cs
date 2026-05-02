using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.AuthRepository
{
    public class AuthRepository(HospitalContext context) : IAuthRepository
    {
        private readonly HospitalContext _context = context;

        public Task<bool> IsEmailNotUniqueAsync(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task RegisterAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
    }
}
