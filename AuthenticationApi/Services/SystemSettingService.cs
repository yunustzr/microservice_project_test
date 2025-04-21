using AuthenticationApi.Infrastructure.Repositories;

namespace AuthenticationApi.Services
{
    public interface ISystemSettingService
    {
        Task<int> GetIntAsync(string key, int defaultValue);
        Task<bool> GetBoolAsync(string key, bool defaultValue);
        Task<string> GetStringAsync(string key, string defaultValue);
    }

    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repo;

        public SystemSettingService(ISystemSettingRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> GetIntAsync(string key, int defaultValue)
        {
            var setting = await _repo.GetByKeyAsync(key);
            return int.TryParse(setting?.Value, out var value) ? value : defaultValue;
        }

        public async Task<bool> GetBoolAsync(string key, bool defaultValue)
        {
            var setting = await _repo.GetByKeyAsync(key);
            return bool.TryParse(setting?.Value, out var value) ? value : defaultValue;
        }

        public async Task<string> GetStringAsync(string key, string defaultValue)
        {
            var setting = await _repo.GetByKeyAsync(key);
            return setting?.Value ?? defaultValue;
        }
    }

}
