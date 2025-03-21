using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Domain.Models.ENTITY;  // Role, Permission, vb. entity'ler
using AuthenticationApi.Infrastructure; 

public interface IRoleRepository : IRepository<Role>
{
    Task<Role> GetRoleWithPoliciesAsync(int roleId);
    Task<Role> GetDefaultRoleAsync();
    Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId);
    Task<List<Role>> GetRolesWithPoliciesByUserIdAsync(Guid userId);
    Task AssignRoleToUserAsync(int roleId, Guid userId);
    Task UnassignRoleFromUserAsync(int roleId, Guid userId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
    Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(int roleId);
}

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role> GetRoleWithPoliciesAsync(int roleId)
    {
        return await _dbSet
            .Include(r => r.RolePolicies)
            .ThenInclude(rp => rp.Policy)
            .ThenInclude(p => p.PolicyPermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<Role> GetDefaultRoleAsync() => await _dbSet.FirstOrDefaultAsync(r => r.IsDefault) ?? throw new InvalidOperationException("Varsayılan rol tanımlı değil!!");
    public async Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId)
    {
        return await _context.RolePolicy
            .Where(rp => rp.RoleId == roleId)
            .SelectMany(rp => rp.Policy.PolicyPermissions)
            .Select(pp => pp.Permission)
            .Distinct()
            .ToListAsync();
    }

     public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
    {
        return await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.User)
            .ToListAsync();
    }
    public async Task<List<Role>> GetRolesWithPoliciesByUserIdAsync(Guid userId)
    {
        return await _context.Role
            .Include(r => r.RolePolicies)
                .ThenInclude(rp => rp.Policy)
                .ThenInclude(p => p.PolicyPermissions)
                .ThenInclude(pp => pp.Permission)
                .ThenInclude(p => p.Resource)
            .Where(r => r.UserRole.Any(ur => ur.UserId == userId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(int roleId)
    {
        // Örneğin: RolePolicy tablosu üzerinden, ilgili role atanan izinleri döndürür.
        return await _context.RolePolicy
            .Where(rp => rp.RoleId == roleId)
            .SelectMany(rp => rp.Policy.PolicyPermissions.Select(pp => pp.Permission))
            .Distinct()
            .ToListAsync();
    }
    public async Task AssignRoleToUserAsync(int roleId, Guid userId)
    {
        var userRole = new UserRoles { RoleId = roleId, UserId = userId, AssignedAt = DateTime.UtcNow };
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task UnassignRoleFromUserAsync(int roleId, Guid userId)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.RoleId == roleId && ur.UserId == userId);
        if (userRole != null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }

}