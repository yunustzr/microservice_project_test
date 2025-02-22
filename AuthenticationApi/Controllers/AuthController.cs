using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationApi.Domain.Models;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthenticationService authenticationService, IUserService userService, IConfiguration config, ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _config = config;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResult = await _authenticationService.AuthenticateAsync(request.Username, request.Password);
            if (authResult.IsAuthenticated)
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new { Token = token });
            }
            _logger.LogWarning("Authentication failed for user: {Username}", request.Username);
            return Unauthorized(new { Message = authResult.Message });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userService.GetByUsernameAsync(request.Username);
            if (existingUser != null)
                return Conflict(new { Message = "User already exists" });

            var user = await _userService.RegisterLocalUserAsync(request);
            return Ok(new { Message = "User registered successfully", User = user });
        }

        private string GenerateJwtToken(string username)
        {
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
}
