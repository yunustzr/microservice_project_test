using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;



public interface IPermissionService
{
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<Permission> GetByIdAsync(int id);
    Task<Permission> CreateAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Role>> GetRolesByPermissionAsync(int permissionId);
}

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    
    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }
    
    public async Task<IEnumerable<Permission>> GetAllAsync() => await _permissionRepository.GetAllAsync();
    
    public async Task<Permission> GetByIdAsync(int id) => await _permissionRepository.GetByIdAsync(id);
    
    public async Task<Permission> CreateAsync(Permission permission)
    {
        await _permissionRepository.AddAsync(permission);
        return permission;
    }
    
    public async Task<Permission> UpdateAsync(Permission permission)
    {
        await _permissionRepository.UpdateAsync(permission);
        return permission;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
            return false;
        await _permissionRepository.RemoveAsync(permission);
        return true;
    }
    
    public async Task<IEnumerable<Role>> GetRolesByPermissionAsync(int permissionId)
        => await _permissionRepository.GetRolesByPermissionAsync(permissionId);
}
