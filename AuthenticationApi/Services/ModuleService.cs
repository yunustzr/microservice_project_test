using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;


public interface IModuleService
{
    Task<IEnumerable<Modules>> GetAllAsync();
    Task<Modules> GetByIdAsync(int id);
    Task<Modules> CreateAsync(Modules module);
    Task<Modules> UpdateAsync(Modules module);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Pages>> GetPagesAsync(int moduleId);
}

public class ModuleService : IModuleService
{
    private readonly IModuleRepository _moduleRepository;
    
    public ModuleService(IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }
    
    public async Task<IEnumerable<Modules>> GetAllAsync() => await _moduleRepository.GetAllAsync();
    
    public async Task<Modules> GetByIdAsync(int id) => await _moduleRepository.GetByIdAsync(id);
    
    public async Task<Modules> CreateAsync(Modules module)
    {
        await _moduleRepository.AddAsync(module);
        return module;
    }
    
    public async Task<Modules> UpdateAsync(Modules module)
    {
        await _moduleRepository.UpdateAsync(module);
        return module;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var module = await _moduleRepository.GetByIdAsync(id);
        if (module == null)
            return false;
        await _moduleRepository.RemoveAsync(module);
        return true;
    }
    
    public async Task<IEnumerable<Pages>> GetPagesAsync(int moduleId)
        => await _moduleRepository.GetPagesAsync(moduleId);
}
