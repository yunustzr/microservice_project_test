using AuthenticationApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationApi.Domain.Models;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILdapService _ldapService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILdapService ldapService, IConfiguration config, ILogger<AuthController> logger)
    {
        _ldapService = ldapService;
        _config = config;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_ldapService.Authenticate(request.Username, request.Password))
        {
            var token = GenerateJwtToken(request.Username);
            return Ok(new { Token = token });
        }

        _logger.LogWarning("LDAP Authentication failed for user: {Username}", request.Username);
        return Unauthorized(new { Message = "LDAP Authentication Failed" });
    }

    private string GenerateJwtToken(string username)
    {
        // JWT ayarlarýný config'den okuyoruz
        var jwtSecretKey = _config["Jwt:SecretKey"];
        var jwtIssuer = _config["Jwt:Issuer"];
        var jwtAudience = _config["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtSecretKey) ||
            string.IsNullOrWhiteSpace(jwtIssuer) ||
            string.IsNullOrWhiteSpace(jwtAudience))
        {
            throw new InvalidOperationException("JWT configuration is missing or invalid.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


