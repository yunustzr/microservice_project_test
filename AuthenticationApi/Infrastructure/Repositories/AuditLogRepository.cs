using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;



public interface IAuditLogRepository : IRepository<AuditLog>
{
}
public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext context) : base(context) { }
}
