using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> GetByIdAsync(int id);
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
    Task AssignRoleToUserAsync(int roleId, Guid userId);
    Task UnassignRoleFromUserAsync(int roleId, Guid userId);
}
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    
    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    
    public async Task<IEnumerable<Role>> GetAllAsync() => await _roleRepository.GetAllAsync();
    
    public async Task<Role> GetByIdAsync(int id) => await _roleRepository.GetByIdAsync(id);
    
    public async Task<Role> CreateAsync(Role role)
    {
        await _roleRepository.AddAsync(role);
        return role;
    }
    
    public async Task<Role> UpdateAsync(Role role)
    {
        await _roleRepository.UpdateAsync(role);
        return role;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if(role == null)
            return false;
        await _roleRepository.RemoveAsync(role);
        return true;
    }
    
    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        => await _roleRepository.GetUsersByRoleAsync(roleId);
    
    public async Task AssignRoleToUserAsync(int roleId, Guid userId)
        => await _roleRepository.AssignRoleToUserAsync(roleId, userId);
    
    public async Task UnassignRoleFromUserAsync(int roleId, Guid userId)
        => await _roleRepository.UnassignRoleFromUserAsync(roleId, userId);
}
