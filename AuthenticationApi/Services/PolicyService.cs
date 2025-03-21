using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;


public interface IPolicyService
{
    Task<IEnumerable<Policy>> GetAllAsync();
    Task<Policy> GetByIdAsync(int id);
    Task<Policy> CreateAsync(Policy policy);
    Task<Policy> UpdateAsync(Policy policy);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Permission>> GetPermissionsAsync(int policyId);
    Task AddPermissionAsync(int policyId, int permissionId);
    Task RemovePermissionAsync(int policyId, int permissionId);
}

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository _policyRepository;
    
    public PolicyService(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }
    
    public async Task<IEnumerable<Policy>> GetAllAsync() => await _policyRepository.GetAllAsync();
    
    public async Task<Policy> GetByIdAsync(int id) => await _policyRepository.GetByIdAsync(id);
    
    public async Task<Policy> CreateAsync(Policy policy)
    {
        await _policyRepository.AddAsync(policy);
        return policy;
    }
    
    public async Task<Policy> UpdateAsync(Policy policy)
    {
        await _policyRepository.UpdateAsync(policy);
        return policy;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var policy = await _policyRepository.GetByIdAsync(id);
        if (policy == null)
            return false;
        await _policyRepository.RemoveAsync(policy);
        return true;
    }
    
    public async Task<IEnumerable<Permission>> GetPermissionsAsync(int policyId)
        => await _policyRepository.GetPermissionsAsync(policyId);
    
    public async Task AddPermissionAsync(int policyId, int permissionId)
        => await _policyRepository.AddPermissionAsync(policyId, permissionId);
    
    public async Task RemovePermissionAsync(int policyId, int permissionId)
        => await _policyRepository.RemovePermissionAsync(policyId, permissionId);
}
