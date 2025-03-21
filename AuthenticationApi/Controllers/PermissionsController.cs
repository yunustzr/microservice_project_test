using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        
        // GET: /api/permissions
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var permissions = await _permissionService.GetAllAsync();
            return Ok(permissions);
        }
        
        // GET: /api/permissions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var permission = await _permissionService.GetByIdAsync(id);
            if (permission == null) return NotFound();
            return Ok(permission);
        }
        
        // POST: /api/permissions
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Permission permission)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdPermission = await _permissionService.CreateAsync(permission);
            return CreatedAtAction(nameof(Get), new { id = createdPermission.Id }, createdPermission);
        }
        
        // PUT: /api/permissions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Permission permission)
        {
            if (id != permission.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedPermission = await _permissionService.UpdateAsync(permission);
            return Ok(updatedPermission);
        }
        
        // DELETE: /api/permissions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _permissionService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // GET: /api/permissions/{id}/roles
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRolesByPermission(int id)
        {
            var roles = await _permissionService.GetRolesByPermissionAsync(id);
            return Ok(roles);
        }
    }
}
