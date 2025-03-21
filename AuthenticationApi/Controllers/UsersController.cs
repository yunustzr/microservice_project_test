using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        // GET: /api/users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        
        // GET: /api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        
        // POST: /api/users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdUser = await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
        }
        
        // PUT: /api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedUser = await _userService.UpdateAsync(user);
            return Ok(updatedUser);
        }
        
        // DELETE: /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // GET: /api/users/{id}/roles
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }
        
        // GET: /api/users/{id}/permissions
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetUserPermissions(Guid id)
        {
            var permissions = await _userService.GetUserPermissionsAsync(id);
            return Ok(permissions);
        }


        
        [HttpGet("{id}/modules")]
        [Authorize]
        public async Task<IActionResult> GetUserModules([FromHeader(Name = "X-Client-IP")] string clientIP,Guid id)
        {
            var modules = await _userService.GetUserModulesAsync(id, clientIP);
            return Ok(modules);
        }
    }
}
