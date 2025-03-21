using AuthenticationApi.Configurations;
using AuthenticationApi.Domain.Models;
using AuthenticationApi.Domain.Models.ENTITY;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationApi.Services
{

    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        RefreshTokens GenerateRefreshToken();
    }


    public class JwtTokenService : ITokenService
    {
        private readonly JwtConfig _config;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtTokenService(IOptions<JwtConfig> config)
        {
            _config = config.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
            Console.WriteLine($"JWT Secret: {_config.Secret}"); // Değerleri kontrol edin
        }


        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("timezone", user.Timezone),
                new Claim("culture", user.Culture)
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            }

            var roles = user.UserRoles?
                .Select(ur => ur.Role?.Name)
                .Where(roleName => !string.IsNullOrEmpty(roleName))
                .ToList();

            if (roles?.Any() == true)
            {
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role!)));
            }
            // Rolleri ekle


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.ExpiryMinutes),
                signingCredentials: creds
            );

            return _tokenHandler.WriteToken(token);
        }

        public bool ValidateRefreshToken(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                return principal != null;
            }
            catch
            {
                return false;
            }
        }

        
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret)),
                ValidateLifetime = false
            };

            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }


        public RefreshTokens GenerateRefreshToken()
        {
            return new RefreshTokens
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(_config.RefreshTokenExpiryDays),
                Created = DateTime.UtcNow
            };
        }


    }
}
