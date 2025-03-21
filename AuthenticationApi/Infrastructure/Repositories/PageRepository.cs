using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface IPageRepository : IRepository<Pages>
{
    Task<IEnumerable<Permission>> GetPermissionsAsync(int pageId);
}

public class PageRepository : Repository<Pages>, IPageRepository
{
    public PageRepository(AppDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Permission>> GetPermissionsAsync(int pageId)
    {
        // Örneğin: Sayfa, modül veya kaynak ilişkisine bağlı olarak izin bilgisi çekilebilir.
        // Burada örnek olarak sayfa oluşturulurken, ilgili modüle bağlı izinler döndürülüyor.
        var page = await _dbSet
            .Include(p => p.Modules)
            .FirstOrDefaultAsync(p => p.Id == pageId);
        if (page?.Modules != null)
        {
            var resourceId = page.Modules.Id; // Örnek; gereksinimlere göre uyarlayın.
            var permissions = await _context.Set<Permission>()
                .Where(p => p.ResourceId == resourceId)
                .ToListAsync();
            return permissions;
        }
        return new List<Permission>();
    }
}
