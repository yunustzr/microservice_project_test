using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;
        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }
        
        // GET: /api/pages
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pages = await _pageService.GetAllAsync();
            return Ok(pages);
        }
        
        // GET: /api/pages/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null) return NotFound();
            return Ok(page);
        }
        
        // POST: /api/pages
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pages page)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdPage = await _pageService.CreateAsync(page);
            return CreatedAtAction(nameof(Get), new { id = createdPage.Id }, createdPage);
        }
        
        // PUT: /api/pages/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Pages page)
        {
            if (id != page.Id)
                return BadRequest("ID uyumsuzluğu");
            var updatedPage = await _pageService.UpdateAsync(page);
            return Ok(updatedPage);
        }
        
        // DELETE: /api/pages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pageService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // GET: /api/pages/{id}/permissions
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(int id)
        {
            var permissions = await _pageService.GetPermissionsAsync(id);
            return Ok(permissions);
        }
    }
}
