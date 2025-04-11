using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _groupService.GetAllAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _groupService.GetByIdAsync(id);
            if (group == null)
                return NotFound();
            return Ok(group);
        }

        [HttpGet("{id}/with-users")]
        public async Task<IActionResult> GetGroupWithUsers(int id)
        {
            var group = await _groupService.GetGroupWithUsersAsync(id);
            if (group == null)
                return NotFound();
            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Group group)
        {
            var createdGroup = await _groupService.CreateAsync(group);
            return CreatedAtAction(nameof(GetById), new { id = createdGroup.Id }, createdGroup);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Group group)
        {
            if (id != group.Id)
                return BadRequest();
            
            await _groupService.UpdateAsync(group);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _groupService.DeleteAsync(id);
            return NoContent();
        }
    }
}
