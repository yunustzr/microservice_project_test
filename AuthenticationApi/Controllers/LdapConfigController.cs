using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("api/settings/ldap-configs")]
    public class LdapConfigController : ControllerBase
    {
        private readonly ILdapConfigurationRepository _repo;
        private readonly ILdapConfigService _configService;

        public LdapConfigController(
            ILdapConfigurationRepository repo,
            ILdapConfigService configService)
        {
            _repo = repo;
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _repo.GetAllEnabledLdapConfigsAsync();
            return Ok(configs);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] LdapConfiguration config)
        {
            config.Id = Guid.NewGuid();
            await _repo.AddAsync(config);
            await _configService.InvalidateCacheAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LdapConfiguration config)
        {
            config.Id = id;
            await _repo.UpdateAsync(config);
            await _configService.InvalidateCacheAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var config = await _repo.GetByIdAsync(id);
            if (config == null)
            {
                return NotFound();
            }

            await _repo.RemoveAsync(config);
            await _configService.InvalidateCacheAsync();
            return Ok();
        }
    }
}
