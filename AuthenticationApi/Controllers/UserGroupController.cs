using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly IUserGroupService _userGroupService;
        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userGroups = await _userGroupService.GetAllAsync();
            return Ok(userGroups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userGroup = await _userGroupService.GetByIdAsync(id);
            if (userGroup == null)
                return NotFound();
            return Ok(userGroup);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserGroup userGroup)
        {
            var createdUserGroup = await _userGroupService.CreateAsync(userGroup);
            return CreatedAtAction(nameof(GetById), new { id = createdUserGroup.Id }, createdUserGroup);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userGroupService.DeleteAsync(id);
            return NoContent();
        }
    }
}
