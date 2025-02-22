using AuthenticationApi.Domain.Models;
using System.Security.Claims;

namespace AuthenticationApi.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }

}
