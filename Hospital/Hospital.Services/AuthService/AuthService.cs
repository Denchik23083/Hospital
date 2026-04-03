using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Requests;
using Hospital.Core.Models.Responce;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Db.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Hospital.Services.AuthService
{
    public class AuthService(HospitalContext context, 
            IConfiguration configuration,
            IMapper mapper,
            ILogger<AuthService> logger) : IAuthService
    {
        private readonly HospitalContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task RegisterAsync(RegisterRequest model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                _logger.LogWarning("User with this {Email} email is already exist", model.Email);
                throw new ConflictException(model.Email);
            }
            
            var mappedUser = _mapper.Map<User>(model);

            mappedUser.PasswordHash = new PasswordHasher<User>()
                .HashPassword(mappedUser, model.Password);

            mappedUser.RoleType = RoleType.User;

            await _context.Users.AddAsync(mappedUser);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Email == model.Email);

            if (user is null || 
                new PasswordHasher <User>().VerifyHashedPassword(user,
                user.PasswordHash, model.Password)
                is PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("User or password is wrong");
                throw new UnauthorizedException("User or password is wrong");
            }

            var token = new TokenResponse
            {
                AccessToken = GetJwtToken(user),
                RefreshToken = await GetRefreshTokenAsync(user)
            };

            return token;
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest model)
        {
            var user = await ValidateRefreshTokenAsync(model.UserId, model.RefreshToken);
            
            var token = new TokenResponse
            {
                AccessToken = GetJwtToken(user),
                RefreshToken = await GetRefreshTokenAsync(user)
            };

            return token;
        }

        private string GetJwtToken(User user)
        {
            var secretKey = _configuration["SecretKey"];

            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);

            var key = new SymmetricSecurityKey(secretKeyBytes);

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.Role, user.RoleType.ToString()),
            };

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<string> GetRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private async Task<User> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null 
                || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh Token is not valid!");
                throw new UnauthorizedException("Not valid!");
            }

            return user;
        }
    }
}
