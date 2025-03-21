using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface IModuleRepository : IRepository<Modules>
{
    Task<IEnumerable<Pages>> GetPagesAsync(int moduleId);
}

public class ModuleRepository : Repository<Modules>, IModuleRepository
{
    public ModuleRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Pages>> GetPagesAsync(int moduleId)
    {
        return await _context.Pages
            .Where(p => p.ModulesId == moduleId)
            .ToListAsync();
    }
}
