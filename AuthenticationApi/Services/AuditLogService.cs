using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;


public interface IAuditLogService
{
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<AuditLog> GetByIdAsync(int id);
}
public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }
    
    public async Task<IEnumerable<AuditLog>> GetAllAsync() => await _auditLogRepository.GetAllAsync();
    
    public async Task<AuditLog> GetByIdAsync(int id) => await _auditLogRepository.GetByIdAsync(id);
}
