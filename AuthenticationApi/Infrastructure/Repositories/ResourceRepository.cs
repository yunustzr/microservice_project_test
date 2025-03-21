using System.Net;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource> GetResourceWithIPRestrictionsAsync(int resourceId);
    Task<IEnumerable<Resource>> GetResourcesByModuleAsync(int moduleId);
    Task<bool> IsIPAllowedForResource(int resourceId, string ipAddress);
}

public class ResourceRepository : Repository<Resource>, IResourceRepository
{
    public ResourceRepository(AppDbContext context) : base(context) { }

    public async Task<Resource> GetResourceWithIPRestrictionsAsync(int resourceId)
    {
        return await _dbSet
            .Include(r => r.IPRestrictions)
            .FirstOrDefaultAsync(r => r.Id == resourceId);
    }

    public async Task<IEnumerable<Resource>> GetResourcesByModuleAsync(int moduleId)
    {
        return await _dbSet
            .Where(r => r.ModuleId == moduleId)
            .ToListAsync();
    }

    public async Task<bool> IsIPAllowedForResource(int resourceId, string ipAddress)
    {
        var resource = await GetResourceWithIPRestrictionsAsync(resourceId);
        return resource.IPRestrictions.Any(ip => 
            ip.IPAddress == ipAddress || 
            IPNetwork.TryParse(ip.Subnet, out var network) && 
            network.Contains(IPAddress.Parse(ipAddress)));
    }
}