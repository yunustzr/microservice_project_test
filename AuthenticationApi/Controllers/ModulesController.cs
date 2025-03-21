using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }
        
        // GET: /api/modules
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modules = await _moduleService.GetAllAsync();
            return Ok(modules);
        }
        
        // GET: /api/modules/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var module = await _moduleService.GetByIdAsync(id);
            if (module == null) return NotFound();
            return Ok(module);
        }
        
        // POST: /api/modules
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Modules module)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdModule = await _moduleService.CreateAsync(module);
            return CreatedAtAction(nameof(Get), new { id = createdModule.Id }, createdModule);
        }
        
        // PUT: /api/modules/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Modules module)
        {
            if (id != module.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedModule = await _moduleService.UpdateAsync(module);
            return Ok(updatedModule);
        }
        
        // DELETE: /api/modules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _moduleService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // GET: /api/modules/{id}/pages
        [HttpGet("{id}/pages")]
        public async Task<IActionResult> GetPages(int id)
        {
            var pages = await _moduleService.GetPagesAsync(id);
            return Ok(pages);
        }
    }
}
