using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("api/settings/system")]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingRepository _repo;

        public SystemSettingController(ISystemSettingRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAllAsync();
            return Ok(all);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] SystemSetting setting)
        {
            var existing = await _repo.GetByKeyAsync(setting.Key);
            if (existing != null)
            {
                existing.Value = setting.Value;
                existing.Description = setting.Description;
                await _repo.UpdateAsync(existing);
            }
            else
            {
                setting.Id = Guid.NewGuid();
                await _repo.AddAsync(setting);
            }

            return Ok();
        }
    }

}
