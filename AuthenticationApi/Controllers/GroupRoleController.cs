using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupRoleController : ControllerBase
    {
        private readonly IGroupRoleService _groupRoleService;
        public GroupRoleController(IGroupRoleService groupRoleService)
        {
            _groupRoleService = groupRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groupRoles = await _groupRoleService.GetAllAsync();
            return Ok(groupRoles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var groupRole = await _groupRoleService.GetByIdAsync(id);
            if (groupRole == null)
                return NotFound();
            return Ok(groupRole);
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetRolesByGroupId(int groupId)
        {
            var roles = await _groupRoleService.GetRolesByGroupIdAsync(groupId);
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GroupRole groupRole)
        {
            var createdGroupRole = await _groupRoleService.CreateAsync(groupRole);
            return CreatedAtAction(nameof(GetById), new { id = createdGroupRole.Id }, createdGroupRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _groupRoleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
