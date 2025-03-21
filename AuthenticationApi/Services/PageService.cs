using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;


public interface IPageService
{
    Task<IEnumerable<Pages>> GetAllAsync();
    Task<Pages> GetByIdAsync(int id);
    Task<Pages> CreateAsync(Pages page);
    Task<Pages> UpdateAsync(Pages page);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Permission>> GetPermissionsAsync(int pageId);
}

public class PageService : IPageService
{
    private readonly IPageRepository _pageRepository;
    
    public PageService(IPageRepository pageRepository)
    {
        _pageRepository = pageRepository;
    }
    
    public async Task<IEnumerable<Pages>> GetAllAsync() => await _pageRepository.GetAllAsync();
    
    public async Task<Pages> GetByIdAsync(int id) => await _pageRepository.GetByIdAsync(id);
    
    public async Task<Pages> CreateAsync(Pages page)
    {
        await _pageRepository.AddAsync(page);
        return page;
    }
    
    public async Task<Pages> UpdateAsync(Pages page)
    {
        await _pageRepository.UpdateAsync(page);
        return page;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var page = await _pageRepository.GetByIdAsync(id);
        if (page == null)
            return false;
        await _pageRepository.RemoveAsync(page);
        return true;
    }
    
    public async Task<IEnumerable<Permission>> GetPermissionsAsync(int pageId)
        => await _pageRepository.GetPermissionsAsync(pageId);
}
