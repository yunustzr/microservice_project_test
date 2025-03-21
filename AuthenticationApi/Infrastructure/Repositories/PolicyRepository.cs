using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface IPolicyRepository : IRepository<Policy>
{
    Task<IEnumerable<Permission>> GetPermissionsAsync(int policyId);
    Task AddPermissionAsync(int policyId, int permissionId);
    Task RemovePermissionAsync(int policyId, int permissionId);
}

public class PolicyRepository : Repository<Policy>, IPolicyRepository
{
    public PolicyRepository(AppDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Permission>> GetPermissionsAsync(int policyId)
    {
        var policy = await _dbSet
            .Include(p => p.PolicyPermissions)
            .ThenInclude(pp => pp.Permission)
            .FirstOrDefaultAsync(p => p.Id == policyId);
        return policy?.PolicyPermissions.Select(pp => pp.Permission) ?? new List<Permission>();
    }
    
    public async Task AddPermissionAsync(int policyId, int permissionId)
    {
        var policyPermission = new PolicyPermissions 
        {
            PolicyId = policyId,
            PermissionId = permissionId,
            CreatedAt = System.DateTime.UtcNow,
            CreatedBy = "system",
            IsActive = 1,
            UpdatedBy = "system"
        };
        _context.Set<PolicyPermissions>().Add(policyPermission);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemovePermissionAsync(int policyId, int permissionId)
    {
        var policyPermission = await _context.Set<PolicyPermissions>()
            .FirstOrDefaultAsync(pp => pp.PolicyId == policyId && pp.PermissionId == permissionId);
        if (policyPermission != null)
        {
            _context.Set<PolicyPermissions>().Remove(policyPermission);
            await _context.SaveChangesAsync();
        }
    }
}
