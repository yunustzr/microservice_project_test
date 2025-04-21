using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface ILdapConfigurationRepository : IRepository<LdapConfiguration>
    {
        Task<List<LdapConfiguration>> GetAllEnabledLdapConfigsAsync();
    }
    public class LdapConfigurationRepository : Repository<LdapConfiguration>, ILdapConfigurationRepository
    {
        public LdapConfigurationRepository(AppDbContext context) : base(context) { }
        public async Task<List<LdapConfiguration>> GetAllEnabledLdapConfigsAsync()
        {
            return await _dbSet
                .Where(x => x.Enabled)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

    }
}
