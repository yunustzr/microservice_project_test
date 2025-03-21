using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using AuthenticationApi.Domain.Models.ENTITY;
using System.Text.Json;
using SharedLibrary.Exceptions;

namespace AuthenticationApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly ILogger _logger;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            IKafkaProducerService kafkaProducerService,
            ILoggerFactory loggerFactory)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _kafkaProducerService = kafkaProducerService;
            _logger = loggerFactory.CreateLogger<AppDbContext>();
        }
        public override async Task<int> SaveChangesAsync(
         CancellationToken cancellationToken = default)
        {
            // .ToList() ekleyerek koleksiyonu materialize ediyoruz.
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            var user = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System";

            foreach (var entry in entries)
            {
                var entityName = entry.Entity.GetType().Name;
                var entityId = entry.Property("Id").CurrentValue.ToString();
                var oldValues = entry.State == EntityState.Modified
                    ? JsonSerializer.Serialize(entry.OriginalValues.ToObject())
                    : null;
                var newValues = entry.State != EntityState.Deleted
                    ? JsonSerializer.Serialize(entry.CurrentValues.ToObject())
                    : null;

                var auditLog = new AuditLog
                {
                    Action = entry.State.ToString(),
                    EntityName = entityName,
                    EntityId = entityId,
                    ChangedBy = user,
                    ChangedAt = DateTime.UtcNow,
                    OldValues = oldValues,
                    NewValues = newValues
                };

                AuditLog.Add(auditLog);
                try
                {
                    await _kafkaProducerService.ProduceAsync("audit_logs",auditLog);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Kafka'ya audit log gönderilirken hata oluştu. Entity: {EntityName}, Id: {EntityId}",
        entityName, entityId);

                     // Özel exception fırlatıyoruz, böylece bu hata üst katmanlara iletilebilir.
                     throw new KafkaPublishException($"Audit log Kafka'ya gönderilemedi. Entity: {entityName}, Id: {entityId}", ex);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }



        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<LoginLog> LoginLog { get; set; }
        public DbSet<Operation> Operation { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Policy> Policy { get; set; }
        public DbSet<PolicyPermissions> PolicyPermissions { get; set; }
        public DbSet<Resource> Resource { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RolePolicy> RolePolicy { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Modules> Modules { get; set; }
        public DbSet<Pages> Pages { get; set; }
        public DbSet<IPRestrictions> IPRestrictions { get; set; }
        public DbSet<TempUser> TempUser { get; set; }
        public DbSet<Condition> Condition { get; set; }
        public DbSet<Attributes> Attributes { get; set; }
        public DbSet<Scope> Scope { get; set; }
        public DbSet<PermissionScope> PermissionScope { get; set; }
        public DbSet<PolicyCondition> PolicyCondition { get; set; }
        public DbSet<PolicyVersion> PolicyVersion { get; set; }
        public DbSet<RoleHierarchy> RoleHierarchie { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Soft Delete Filter
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

        }


    }
}
