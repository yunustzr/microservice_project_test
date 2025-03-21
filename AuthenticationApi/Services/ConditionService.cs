using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;


public interface IConditionService
{
    Task<IEnumerable<Condition>> GetAllAsync();
    Task<Condition> GetByIdAsync(int id);
    Task<Condition> CreateAsync(Condition condition);
    Task<Condition> UpdateAsync(Condition condition);
    Task<bool> DeleteAsync(int id);
}
public class ConditionService : IConditionService
{
    private readonly IConditionRepository _conditionRepository;
    
    public ConditionService(IConditionRepository conditionRepository)
    {
        _conditionRepository = conditionRepository;
    }
    
    public async Task<IEnumerable<Condition>> GetAllAsync() => await _conditionRepository.GetAllAsync();
    
    public async Task<Condition> GetByIdAsync(int id) => await _conditionRepository.GetByIdAsync(id);
    
    public async Task<Condition> CreateAsync(Condition condition)
    {
        await _conditionRepository.AddAsync(condition);
        return condition;
    }
    
    public async Task<Condition> UpdateAsync(Condition condition)
    {
        await _conditionRepository.UpdateAsync(condition);
        return condition;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var condition = await _conditionRepository.GetByIdAsync(id);
        if (condition == null)
            return false;
        await _conditionRepository.RemoveAsync(condition);
        return true;
    }
}
