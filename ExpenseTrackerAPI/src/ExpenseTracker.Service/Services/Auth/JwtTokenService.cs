using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Service.Services.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = "expense-tracker";
        public string Audience { get; set; } = "expense-tracker";
        public string Secret { get; set; } = "replace_this_with_env_secret";
        public int AccessMinutes { get; set; } = 60;
        public int RefreshDays { get; set; } = 30;
    }

    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(JwtOptions options)
        {
            _options = options;
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: new[] { new Claim("sub", user.Id.ToString()), new Claim("email", user.Email) },
                notBefore: now,
                expires: now.AddMinutes(_options.AccessMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        public string HashRefreshToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
