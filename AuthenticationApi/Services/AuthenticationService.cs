using AuthenticationApi.Domain.Models.ENTITY;
using System.Security.Authentication;
using AuthenticationApi.Domain.Models.DTO;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthenticationApi.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(LoginRequest request);
        //Task<AuthenticationResult> RegisterAsync(RegisterRequest request);
        Task RegisterTempAsync(RegisterRequest request);

        Task<List<ModuleDto>> GetUserModulesAsync(Guid userId, string clientIP);
        Task<AuthenticationResult> VerifyEmailAsync(EmailVerifyRequest emailVerifyRequest);
        //Task<User> GetByUsernameAsync(RegisterRequest request);
        Task<AuthenticationResult> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
        Task RevokeAllTokensAsync(Guid userId);
        //Task<AuthenticationResult> RefreshTokenAsync(string token);
        //Task RevokeTokenAsync(string token);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTempRepository _userTempRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IResourceRepository _resourceRepository;
        private readonly ILdapService _ldapService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ILoginLogService _loginLogService;

        private readonly IHttpContextAccessor _httpContextAccessor; 



        public AuthenticationService(
            IUserRepository userRepository,
            IUserTempRepository userTempRepository,
            IRoleRepository roleRepository,
            PasswordHasher passwordHasher,
            ITokenService tokenService,
            IResourceRepository resourceRepository,
            ILdapService ldapService,
            IEmailService emailService,
            ILogger<AuthenticationService> logger,
            ILoginLogService loginLogService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userTempRepository = userTempRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _resourceRepository = resourceRepository;
            _ldapService = ldapService;
            _emailService = emailService;
            _logger = logger;
            _loginLogService = loginLogService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<AuthenticationResult> LoginAsync(LoginRequest request)
        {
            // Giriş bilgilerini kaydetmek için ip ve device bilgisi alınıyor.
            var httpContext = _httpContextAccessor.HttpContext;
            string ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            string deviceInfo = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            try
            {
                var existingUser = (await _userRepository.FindAsync(u => u.Email == request.Email)).ToList().FirstOrDefault();
                if (existingUser == null)
                {
                    throw new AuthenticationException($"Kullanıcı bulunamadı");
                }
                if (!existingUser.IsActive)
                {
                    throw new AuthenticationException($"Aktif olmayan kullanıcı girişi ");
                }


                var authenticatedUser = await _ldapService.AuthenticateAsync(request);


                // Hesap kilitleme kontrolü
                if (authenticatedUser.LockoutEnd.HasValue && authenticatedUser.LockoutEnd > DateTime.UtcNow)
                {
                    throw new AuthenticationException($"Account locked until {authenticatedUser.LockoutEnd}");
                }


                // Başarılı giriş log kaydı oluşturma
                var successLog = new LoginLog
                {
                    Username = authenticatedUser.Email,
                    LoginTime = DateTime.UtcNow,
                    IPAddress = ipAddress,
                    DeviceInfo = deviceInfo,
                    IsSuccessful = true,
                    FailureReason = null
                };
                await _loginLogService.LogAsync(successLog);

                return await GenerateAuthenticationResult(authenticatedUser);
            }
            catch (Exception ex)
            {
                // Hata durumunda login log kaydı oluşturma (başarısız giriş)
                var failLog = new LoginLog
                {
                    Username = request.Email,
                    LoginTime = DateTime.UtcNow,
                    IPAddress = ipAddress,
                    DeviceInfo = deviceInfo,
                    IsSuccessful = false,
                    FailureReason = ex.Message
                };
                await _loginLogService.LogAsync(failLog);
                _logger.LogError(ex, "Login failed for {Email}", request.Email);
                throw;
            }
        }


        public async Task RegisterTempAsync(RegisterRequest request)
        {
            try
            {
                // 1. Ana kullanıcı tablosunda verilen e-posta ile kayıt var mı kontrol edelim
                var existingMainUser = (await _userRepository.FindAsync(u => u.Email == request.Email))
                    .ToList().FirstOrDefault();
                if (existingMainUser != null)
                {
                    throw new AuthenticationException("Bu Email ile kullanıcı zaten kayıtlı.");
                }

                // 2. Temp kullanıcı tablosunda (silinmemiş kayıtlar) verilen e-posta ile kayıt var mı kontrol edelim
                var existingTempUser = (await _userTempRepository.FindAsync(u => u.Email == request.Email && !u.IsDeleted))
                    .ToList().FirstOrDefault();

                if (existingTempUser != null)
                {
                    // Eğer token süresi geçmişse, yeni token üret ve güncelle
                    if (existingTempUser.ExpiresAt < DateTime.UtcNow)
                    {
                        var newVerificationCode = new Random().Next(100000, 999999).ToString();
                        existingTempUser.VerificationCode = newVerificationCode;
                        existingTempUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10); // Yeni token 10 dakika geçerli
                        await _userTempRepository.UpdateAsync(existingTempUser);
                        await _emailService.SendVerificationEmailAsync(existingTempUser.Email, newVerificationCode);

                        throw new AuthenticationException("Bu Email ile kullanıcı daha önce kayıt oluşturmuş. Token süresi dolmuş, yeni doğrulama kodu e-mail'e gönderildi.");
                    }
                    else
                    {
                        // Eğer token henüz geçerli ise
                        throw new AuthenticationException("Bu Email ile kullanıcı daha önce kayıt oluşturmuş.");
                    }
                }

                // 3. Eğer hem ana hem de temp tablolarda kayıt yoksa, yeni bir temp kullanıcı oluştur
                var verificationCode = new Random().Next(100000, 999999).ToString();
                var tempNewUser = new TempUser
                {
                    UserName = request.Username,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = _passwordHasher.HashPassword(request.Password),
                    VerificationCode = verificationCode,
                    AcceptPrivacyPolicy = request.AcceptPrivacyPolicy,
                    AcceptTerms = request.AcceptTerms,
                    // ExpiresAt alanı TempUser modelinde default olarak 15 dakika sonrasını alıyorsa, burada da ayarlanabilir.
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                };

                await _userTempRepository.AddAsync(tempNewUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Username}", request.Username);
                throw;
            }
        }
        public async Task<AuthenticationResult> VerifyEmailAsync(EmailVerifyRequest emailVerifyRequest)
        {
            try
            {
                // Token'e göre, silinmemiş (active) temp kullanıcıyı getiriyoruz
                var tempUser = (await _userTempRepository.FindAsync(u => u.VerificationCode == emailVerifyRequest.Token && u.Email == emailVerifyRequest.Email && !u.IsDeleted)).ToList().FirstOrDefault();

                if (tempUser == null)
                {
                    throw new AuthenticationException("Geçersiz token.");
                }

                // Eğer token süresi dolmuşsa, yeni token üret ve güncelle
                if (tempUser.ExpiresAt < DateTime.UtcNow)
                {
                    var newVerificationCode = new Random().Next(100000, 999999).ToString();
                    tempUser.VerificationCode = newVerificationCode;
                    tempUser.ExpiresAt = DateTime.UtcNow.AddMinutes(15); // Yeni token 15 dakika geçerli
                    await _userTempRepository.UpdateAsync(tempUser);

                    // Opsiyonel: Yeni doğrulama kodunu kullanıcının e-posta adresine gönder
                    await _emailService.SendVerificationEmailAsync(tempUser.Email, newVerificationCode);

                    throw new AuthenticationException("Token süresi doldu. Yeni doğrulama kodu gönderildi.");
                }

                // Token geçerliyse, tempUser kaydını soft delete yap (IsDeleted = true)
                tempUser.IsDeleted = true;
                await _userTempRepository.UpdateAsync(tempUser);

                // TempUser verilerini kullanarak ana User kaydını oluştur
                var newUser = await CreateLocalUser(tempUser);
                await AssignDefaultRole(newUser);

                // Başarılı kullanıcı doğrulama sonucunu döndür
                return await GenerateAuthenticationResult(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email doğrulama işlemi başarısız. Token: {Token}", emailVerifyRequest.Token);
                throw;
            }
        }



        private async Task<AuthenticationResult> GenerateAuthenticationResult(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            
            user.RefreshTokens.Add(refreshToken);
            await _userRepository.UpdateAsync(user);

            return new AuthenticationResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    NormalizedUserName = user.NormalizedUserName,
                    NormalizedLastName = user.NormalizedLastName,                    
                    TokenVersion = user.TokenVersion,
                    IsLdapUser = user.IsLdapUser,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    LockoutEnd = user.LockoutEnd,
                    Timezone = user.Timezone,
                    Culture = user.Culture
                }
            };


        }


        public async Task<AuthenticationResult> RefreshTokenAsync(string token)
        {
            try
            {
                // Refresh token'ı bul ve doğrula
                var user = await _userRepository.GetUserByRefreshTokenAsync(token)
                    ?? throw new AuthenticationException("Invalid refresh token");

                var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == token)
                    ?? throw new AuthenticationException("Invalid refresh token");

                // Token süresi kontrolü
                if (existingToken.Expires < DateTime.UtcNow)
                {
                    throw new AuthenticationException("Refresh token expired");
                }

                // Token rotation uygula
                user.RefreshTokens.Remove(existingToken);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);

                // Eski token'ı revoke et
                existingToken.Revoked = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                // Yeni token'ları oluştur
                return await GenerateAuthenticationResult(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token failed");
                throw;
            }
        }

        public async Task RevokeTokenAsync(string token)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync(token)
                    ?? throw new AuthenticationException("Invalid refresh token");

                var tokenToRevoke = user.RefreshTokens.FirstOrDefault(t => t.Token == token)
                    ?? throw new AuthenticationException("Token not found");

                tokenToRevoke.Revoked = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                // Logout zamanını LoginLog tablosuna kaydet
                await _loginLogService.UpdateLogoutTimeForUserAsync(user.Email, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token revocation failed");
                throw;
            }
        }

        public async Task RevokeAllTokensAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId)
                    ?? throw new AuthenticationException("User not found");

                foreach (var token in user.RefreshTokens.Where(t => t.Revoked == null))
                {
                    token.Revoked = DateTime.UtcNow;
                }

                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Revoke all tokens failed");
                throw;
            }
        }


        public async Task<List<ModuleDto>> GetUserModulesAsync(Guid userId, string clientIP)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId)
                    ?? throw new AuthenticationException("User not found");

                // Kullanıcının rollerini ve ilişkili politikaları getir
                var roles = await _roleRepository.GetRolesWithPoliciesByUserIdAsync(userId);

                // Tüm izinleri topla
                var permissions = roles
                    .SelectMany(r => r.RolePolicies)
                    .SelectMany(rp => rp.Policy.PolicyPermissions)
                    .Select(pp => pp.Permission)
                    .ToList();

                // İzinlerden kaynakları getir
                var resources = permissions
                    .Select(p => p.Resource)
                    .Where(r => r != null)
                    .Distinct()
                    .ToList();

                // IP filtrelemesi uygula
                var filteredResources = FilterResourcesByIP(resources, clientIP);

                // Modül ve sayfa yapısını oluştur
                return MapToModuleDto(filteredResources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get modules for user {UserId}", userId);
                throw;
            }
        }

        private List<ModuleDto> MapToModuleDto(List<Resource> resources)
        {
            return resources
                .Where(r => r.Module != null)
                .GroupBy(r => r.Module)
                .Select(g => new ModuleDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Icon = g.Key.Icon,
                    OrderIndex = g.Key.OrderIndex,
                    Pages = g.Where(r => r.Page != null)
                            .Select(r => new PageDto
                            {
                                Id = r.Page.Id,
                                Name = r.Page.Name,
                                RoutePath = r.Page.RoutePath,
                                IPRestrictions = r.IPRestrictions.Select(ip => new IPRestrictionDto
                                {
                                    IPAddress = ip.IPAddress,
                                    Subnet = ip.Subnet,
                                    IsAllowed = ip.IsAllowed
                                }).ToList()
                            }).Distinct().ToList()
                }).OrderBy(m => m.OrderIndex).ToList();
        }





        private async Task<User?> CreateLocalUser(TempUser tempUser)
        {
            var newUser = new User
            {
                UserName = tempUser.UserName,
                LastName = tempUser.LastName,
                NormalizedUserName = tempUser.UserName.ToUpperInvariant(),
                NormalizedLastName = tempUser.LastName.ToUpperInvariant(),
                Email = tempUser.Email,
                NormalizedEmail = tempUser.Email.ToUpperInvariant(),
                PasswordHash = tempUser.PasswordHash.ToString(),
                IsLdapUser = false,
                CreatedAt = tempUser.CreatedAt,
                TermsAcceptedAt = tempUser.CreatedAt,
                PrivacyPolicyAcceptedAt = tempUser.CreatedAt,
                IsActive = true,
                RefreshTokens = new List<RefreshTokens>()
            };
            return await _userRepository.AddAsync(newUser);
        }

        private async Task AssignDefaultRole(User user)
        {

            var defaultRole = await _roleRepository.GetDefaultRoleAsync();

            await _userRepository.AssignRoleToUserAsync(user.Id, defaultRole.Id);

        }



        private List<Resource> FilterResourcesByIP(List<Resource> resources, string clientIP)
        {
            return resources.Where(resource =>
            {
                var restrictions = resource.IPRestrictions;
                if (restrictions == null || !restrictions.Any()) return true;

                return restrictions.Any(r =>
                (IPAddress.TryParse(r.IPAddress, out var ip) && ip.Equals(IPAddress.Parse(clientIP))) ||
                (IPNetwork.TryParse(r.Subnet, out var network) && network.Contains(IPAddress.Parse(clientIP))));
            }).ToList();
        }

        private AuthenticationResult MapToAuthenticationResult(User user, string token, RefreshTokens refreshToken)
        {
            return new AuthenticationResult
            {
                AccessToken = token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = MapToUserDto(user)
            };
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                NormalizedUserName = user.NormalizedUserName,
                NormalizedLastName = user.NormalizedLastName,
                Email = user.Email,
                TokenVersion = user.TokenVersion,
                IsLdapUser = user.IsLdapUser,
                FailedLoginAttempts = user.FailedLoginAttempts,
                LockoutEnd = user.LockoutEnd,
                Timezone = user.Timezone,
                Culture = user.Culture                
            };
        }







    }
}
