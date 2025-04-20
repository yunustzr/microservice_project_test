using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace AuthenticationApi.Services
{
    public interface ILdapConfigService
    {
        Task<IReadOnlyList<LdapConfiguration>> GetLdapConfigurationsAsync();
        Task InvalidateCacheAsync();
    }

    public class LdapConfigService : ILdapConfigService
    {
        private readonly IMemoryCache _cache;
        private readonly ILdapConfigurationRepository _repo;
        private readonly ILogger<LdapConfigService> _logger;
        private const string CacheKey = "LdapConfigurations";
        private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public LdapConfigService(
            IMemoryCache cache,
            ILdapConfigurationRepository repo,
            ILogger<LdapConfigService> logger)
        {
            _cache = cache;
            _repo = repo;
            _logger = logger;
        }

        public async Task<IReadOnlyList<LdapConfiguration>> GetLdapConfigurationsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                _logger.LogInformation("[LDAP Config] Cache refreshed from database");
                return await _repo.GetAllEnabledLdapConfigsAsync();
            });
        }

        public Task InvalidateCacheAsync()
        {
            _cache.Remove(CacheKey);
            return Task.CompletedTask;
        }
    }
}
