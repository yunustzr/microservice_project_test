using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConditionsController : ControllerBase
    {
        private readonly IConditionService _conditionService;
        public ConditionsController(IConditionService conditionService)
        {
            _conditionService = conditionService;
        }
        
        // GET: /api/conditions
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var conditions = await _conditionService.GetAllAsync();
            return Ok(conditions);
        }
        
        // GET: /api/conditions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var condition = await _conditionService.GetByIdAsync(id);
            if (condition == null) return NotFound();
            return Ok(condition);
        }
        
        // POST: /api/conditions
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Condition condition)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdCondition = await _conditionService.CreateAsync(condition);
            return CreatedAtAction(nameof(Get), new { id = createdCondition.Id }, createdCondition);
        }
        
        // PUT: /api/conditions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Condition condition)
        {
            if (id != condition.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedCondition = await _conditionService.UpdateAsync(condition);
            return Ok(updatedCondition);
        }
        
        // DELETE: /api/conditions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _conditionService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
