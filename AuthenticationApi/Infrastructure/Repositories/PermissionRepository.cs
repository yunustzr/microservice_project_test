using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;



public interface IPermissionRepository : IRepository<Permission>
{
    Task<IEnumerable<Role>> GetRolesByPermissionAsync(int permissionId);
}
public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Role>> GetRolesByPermissionAsync(int permissionId)
    {
        // Örneğin: İlgili izin, RolePolicy üzerinden dolaylı olarak rollere atanmışsa
        var roles = await _context.RolePolicy
            .Where(rp => rp.Policy.PolicyPermissions.Any(pp => pp.PermissionId == permissionId))
            .Select(rp => rp.Role)
            .Distinct()
            .ToListAsync();
        return roles;
    }
}
