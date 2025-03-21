using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        
        // GET: /api/roles
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }
        
        // GET: /api/roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }
        
        // POST: /api/roles
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Role role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdRole = await _roleService.CreateAsync(role);
            return CreatedAtAction(nameof(Get), new { id = createdRole.Id }, createdRole);
        }
        
        // PUT: /api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Role role)
        {
            if (id != role.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedRole = await _roleService.UpdateAsync(role);
            return Ok(updatedRole);
        }
        
        // DELETE: /api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // GET: /api/roles/{id}/users
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersByRole(int id)
        {
            var users = await _roleService.GetUsersByRoleAsync(id);
            return Ok(users);
        }
        
        // POST: /api/roles/{id}/assign  
        // (Body'da userId gönderiliyor)
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] Guid userId)
        {
            await _roleService.AssignRoleToUserAsync(id, userId);
            return NoContent();
        }
        
        // DELETE: /api/roles/{id}/unassign?userId={userId}
        [HttpDelete("{id}/unassign")]
        public async Task<IActionResult> UnassignRole(int id, [FromQuery] Guid userId)
        {
            await _roleService.UnassignRoleFromUserAsync(id, userId);
            return NoContent();
        }
    }
}
