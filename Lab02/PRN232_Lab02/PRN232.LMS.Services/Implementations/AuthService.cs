using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Auth;

namespace PRN232.LMS.Services.Implementations
{
    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
    {
        private const int AccessTokenExpiresInSeconds = 3600;
        private static readonly PasswordHasher<User> PasswordHasher = new();

        public async Task<AuthTokenResponse?> LoginAsync(LoginRequest request)
        {
            var user = await unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return null;
            }

            var result = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await IssueTokenAsync(user);
        }

        public async Task<AuthTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var now = GetDatabaseUtcNow();
            var token = await unitOfWork.RefreshTokens.FirstOrDefaultAsync(t =>
                t.Token == request.RefreshToken &&
                !t.IsRevoked &&
                t.ExpiresAt > now);

            if (token == null)
            {
                return null;
            }

            var user = await unitOfWork.Users.GetByIdAsync(token.UserId);
            if (user == null)
            {
                return null;
            }

            token.IsRevoked = true;
            unitOfWork.RefreshTokens.Update(token);

            return await IssueTokenAsync(user);
        }

        private async Task<AuthTokenResponse> IssueTokenAsync(User user)
        {
            var now = GetDatabaseUtcNow();
            var refreshToken = GenerateRefreshToken();
            await unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                CreatedAt = now,
                ExpiresAt = now.AddDays(7),
                IsRevoked = false
            });
            await unitOfWork.SaveChangesAsync();

            return new AuthTokenResponse
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = refreshToken,
                ExpiresIn = AccessTokenExpiresInSeconds
            };
        }

        private string GenerateAccessToken(User user)
        {
            var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret is not configured.");
            var issuer = configuration["Jwt:Issuer"] ?? "PRN232.LMS.API";
            var audience = configuration["Jwt:Audience"] ?? "PRN232.LMS.Client";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(AccessTokenExpiresInSeconds),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static DateTime GetDatabaseUtcNow()
        {
            return DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
