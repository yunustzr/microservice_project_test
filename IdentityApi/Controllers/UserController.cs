using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] Users = new[]
        {
            "Yunus", "Beyza Nur"
        };

        
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Users);
        }
    }
}
