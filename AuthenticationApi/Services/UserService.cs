using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.DTO;
using AuthenticationApi.Domain.Models.ENTITY;

namespace AuthenticationApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
        Task<List<ModuleDto>> GetUserModulesAsync(Guid userId, string clientIP);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<AuthenticationService> _logger;
        public UserService(IUserRepository userRepository,IRoleRepository roleRepository,ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
            return await _userRepository.GetByIdAsync(user.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(userId);
            return user?.UserRoles.Select(ur => ur.Role) ?? new List<Role>();
        }

        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId)
        {
            var roles = await GetUserRolesAsync(userId);
            var permissions = new List<Permission>();
            foreach (var role in roles)
            {
                var rolePermissions = await _roleRepository.GetPermissionsByRoleAsync(role.Id);
                permissions.AddRange(rolePermissions);
            }
            return permissions.Distinct();
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



    }
}
