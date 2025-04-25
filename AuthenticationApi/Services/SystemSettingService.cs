using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;

namespace AuthenticationApi.Services
{
    public interface ISystemSettingService
    {
        Task<int> GetIntAsync(string key, int defaultValue);
        Task<bool> GetBoolAsync(string key, bool defaultValue);
        Task<string> GetStringAsync(string key, string defaultValue);
        Task CreateOrUpdateSettingAsync(SystemSetting setting);
        Task<SystemSetting> GetSettingAsync(string key);
    }

    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;

        public SystemSettingService(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> GetIntAsync(string key, int defaultValue)
        {
            var setting = await _repository.GetByKeyAsync(key);
            return int.TryParse(setting?.Value, out var value) ? value : defaultValue;
        }

        public async Task<bool> GetBoolAsync(string key, bool defaultValue)
        {
            var setting = await _repository.GetByKeyAsync(key);
            return bool.TryParse(setting?.Value, out var value) ? value : defaultValue;
        }

        public async Task<string> GetStringAsync(string key, string defaultValue)
        {
            var setting = await _repository.GetByKeyAsync(key);
            return setting?.Value ?? defaultValue;
        }

        public async Task CreateOrUpdateSettingAsync(SystemSetting setting)
        {
            var existing = await _repository.GetByKeyAsync(setting.Key);

            if (existing != null)
            {
                existing.Value = setting.Value;
                existing.Description = setting.Description;
                existing.UpdatedAt = DateTime.UtcNow;
                _repository.UpdateAsync(existing);
            }
            else
            {
                await _repository.AddAsync(setting);
            }

        }
 
        public async Task<SystemSetting> GetSettingAsync(string key)
        {
            return await _repository.GetByKeyAsync(key);
        }
        
    }

}
