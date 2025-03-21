using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Services;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }
        
        // GET: /api/audit-logs
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var logs = await _auditLogService.GetAllAsync();
            return Ok(logs);
        }
        
        // GET: /api/audit-logs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var log = await _auditLogService.GetByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }
    }
}
