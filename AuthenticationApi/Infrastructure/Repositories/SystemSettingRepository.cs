using AuthenticationApi.Domain.Models.ENTITY;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface ISystemSettingRepository : IRepository<SystemSetting>
    {
        Task<SystemSetting> GetByKeyAsync(string key);
    }
    public class SystemSettingRepository : Repository<SystemSetting>, ISystemSettingRepository
    {
        public SystemSettingRepository(AppDbContext context) : base(context) { }

        public async Task<SystemSetting> GetByKeyAsync(string key)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Key == key);
        }
    }

}
