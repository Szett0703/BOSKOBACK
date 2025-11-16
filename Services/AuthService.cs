using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DBTest_BACK.Models;
using DBTest_BACK.DTOs;

namespace DBTest_BACK.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        string GenerateResetToken();
        Task<bool> ValidateGoogleToken(string googleToken);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!)
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role),
                new Claim("provider", user.Provider),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Agregar claim de rol para .NET Authorization
            claims.Add(new Claim(ClaimTypes.Role, user.Role));

            if (!string.IsNullOrEmpty(user.Phone))
            {
                claims.Add(new Claim("phone", user.Phone));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:ExpirationMinutes"])
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateResetToken()
        {
            // Generar token aleatorio seguro de 32 bytes
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<bool> ValidateGoogleToken(string googleToken)
        {
            try
            {
                var validationSettings = new Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["GoogleAuth:ClientId"]! }
                };

                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(
                    googleToken,
                    validationSettings
                );

                return payload != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
