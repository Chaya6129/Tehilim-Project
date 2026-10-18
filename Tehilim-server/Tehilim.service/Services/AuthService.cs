using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Interfaces.Services;
using Tehilim.Core.Results;

namespace Tehilim.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher<User> _hasher;
        private readonly JwtSettings _jwt;

        public AuthService(IUserRepository users, IPasswordHasher<User> hasher, IOptions<JwtSettings> jwt)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt.Value;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var email = NormalizeEmail(request.Email);

            if (await _users.EmailExistsAsync(email, ct))
                return Result<AuthResponse>.Conflict("כתובת האימייל כבר רשומה במערכת");

            var user = new User
            {
                DisplayName = request.FullName.Trim(),
                Email = email,
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _users.AddAsync(user, ct);
            await _users.SaveChangesAsync(ct);

            return Result<AuthResponse>.Success(CreateAuthResponse(user));
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(NormalizeEmail(request.Email), ct);

            if (user is null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                return Result<AuthResponse>.Unauthorized("אימייל או סיסמה שגויים");

            return Result<AuthResponse>.Success(CreateAuthResponse(user));
        }

        private AuthResponse CreateAuthResponse(User user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                Expires = expiresAt,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return new AuthResponse(token, expiresAt, user.DisplayName, user.Role.ToString());
        }

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}