using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;
using AuthenticationApi.Domain.Models;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AuthenticationApi.Domain.Models.DTO;
using AuthenticationApi.Domain.Exceptions;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authService;
        private readonly ILdapService _ldapService;
        private readonly ITokenService _tokenService;

        public AuthController(
            IAuthenticationService authService,
            ILdapService ldapService,
            ITokenService tokenService)
        {
            _authService = authService;
            _ldapService = ldapService;
            _tokenService = tokenService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblemModel();

            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                // Burada ex.FailedLoginAttempts zaten serviste artırıldı ve persist edildi
                var pd = new ProblemDetails
                {
                    Title    = "Authentication Failed",
                    Detail   = ex.Message,
                    Status   = StatusCodes.Status401Unauthorized,
                    Instance = HttpContext.Request.Path
                };

                pd.Extensions["failedLoginAttempts"] = ex.FailedLoginAttempts;
                pd.Extensions["remainingAttempts"]   = ex.RemainingAttempts;
                // 3 veya daha fazla başarısız denemedeyse front-end Captcha göstersin
                pd.Extensions["showCaptcha"]        = ex.FailedLoginAttempts >= 3;

                return Unauthorized(pd);
            }
            catch (AuthenticationException ex)
            {
                // Diğer authentication hatalarında (kilitli, inaktif vs.)
                return Problem500("Authentication Error", ex.Message);
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                //var result = await _authService.GetByUsernameAsync(request);
                await _authService.RegisterTempAsync(request);
                return Ok("Kayıt başarılı. Lütfen emailinizi doğrulayın.");
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("email-verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerifyRequest emailVerifyRequest)
        {
            try
            {
                var result = await _authService.VerifyEmailAsync(emailVerifyRequest);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isAvailable = await _authService.IsEmailAvailableAsync(request.Email);

            if (!isAvailable)
            {
                return BadRequest("Bu e-posta adresi ile zaten kayıtlı bir kullanıcı var.");
            }

            return Ok("E-posta adresi kullanılabilir.");
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            await _authService.RevokeTokenAsync(request.RefreshToken);
            return NoContent();
        }

        [Authorize]
        [HttpPost("revoke-all")]
        public async Task<IActionResult> RevokeAllTokens()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _authService.RevokeAllTokensAsync(userId);
            return NoContent();
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            // Kullanıcının refresh token'ını al
            var refreshToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Refresh token'ı geçersiz kıl ve logout zamanını kaydet
            await _authService.RevokeTokenAsync(refreshToken);

            return Ok(new { Message = "Logout successful." });

        }

    }
}