using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.AuthRepository
{
    public class AuthRepository(HospitalContext context) : IAuthRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<bool> IsEmailNotUniqueAsync(string email, CancellationToken ct)
        {
            return await _context.Users.AnyAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetUserAsync(int id, CancellationToken ct)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task RegisterAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
        }
    }
}
