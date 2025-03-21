using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService _policyService;
        public PoliciesController(IPolicyService policyService)
        {
            _policyService = policyService;
        }
        
        // GET: /api/policies
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var policies = await _policyService.GetAllAsync();
            return Ok(policies);
        }
        
        // GET: /api/policies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var policy = await _policyService.GetByIdAsync(id);
            if (policy == null) return NotFound();
            return Ok(policy);
        }
        
        // POST: /api/policies
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Policy policy)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdPolicy = await _policyService.CreateAsync(policy);
            return CreatedAtAction(nameof(Get), new { id = createdPolicy.Id }, createdPolicy);
        }
        
        // PUT: /api/policies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Policy policy)
        {
            if (id != policy.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedPolicy = await _policyService.UpdateAsync(policy);
            return Ok(updatedPolicy);
        }
        
        // DELETE: /api/policies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _policyService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // GET: /api/policies/{id}/permissions
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(int id)
        {
            var permissions = await _policyService.GetPermissionsAsync(id);
            return Ok(permissions);
        }
        
        // POST: /api/policies/{id}/add-permission  
        // (Body'da permissionId gönderiliyor)
        [HttpPost("{id}/add-permission")]
        public async Task<IActionResult> AddPermission(int id, [FromBody] int permissionId)
        {
            await _policyService.AddPermissionAsync(id, permissionId);
            return NoContent();
        }
        
        // DELETE: /api/policies/{id}/remove-permission?permissionId={permissionId}
        [HttpDelete("{id}/remove-permission")]
        public async Task<IActionResult> RemovePermission(int id, [FromQuery] int permissionId)
        {
            await _policyService.RemovePermissionAsync(id, permissionId);
            return NoContent();
        }
    }
}
