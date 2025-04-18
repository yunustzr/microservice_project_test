using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using AuthenticationApi.Domain.Models.ENTITY;
using System.Text.Json;
using SharedLibrary.Exceptions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<AuditLog>();
            var user = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System";
            var utcNow = DateTime.UtcNow;

            // 1. Audit Log'ları Oluştur
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || 
                           e.State == EntityState.Modified || 
                           e.State == EntityState.Deleted))
            {
                var auditEntry = CreateAuditEntry(entry, user, utcNow);
                auditEntries.Add(auditEntry);
            }

            // 2. Transaction'ı Başlat
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 3. Ana İşlemleri Kaydet
                var result = await base.SaveChangesAsync(cancellationToken);

                // 4. Audit Log'ları Kaydet (Outbox Pattern)
                await SaveAuditLogsAsync(auditEntries, cancellationToken);

                // 5. Kafka'ya Asenkron Gönderim
                _ = Task.Run(async () => 
                {
                    try
                    {
                        await _kafkaProducerService.ProduceBulkAsync("audit_logs", auditEntries);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Kafka bulk audit log gönderim hatası");
                    }
                }, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    
        private AuditLog CreateAuditEntry(EntityEntry entry, string user, DateTime timestamp)
        {
            return new AuditLog
            {
                ServiceName = "AuthenticationApi",
                Action = entry.State.ToString(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.Property("Id").CurrentValue?.ToString(),
                ChangedBy = user,
                ChangedAt = timestamp,
                OldValues = entry.State == EntityState.Modified 
                    ? JsonSerializer.Serialize(entry.OriginalValues.ToObject()) 
                    : null,
                NewValues = entry.State != EntityState.Deleted 
                    ? JsonSerializer.Serialize(entry.CurrentValues.ToObject()) 
                    : null
            };
        }

        private async Task SaveAuditLogsAsync(List<AuditLog> auditEntries, CancellationToken cancellationToken)
        {
            // Audit log'ları geçici olarak takip etmeyi devre dışı bırak
            ChangeTracker.AutoDetectChangesEnabled = false;

            foreach (var log in auditEntries)
            {
                await Set<AuditLog>().AddAsync(log, cancellationToken);
            }

            await base.SaveChangesAsync(cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;
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
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
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
        public DbSet<Group> Group {get;set;}
        public DbSet<GroupRole> GroupsRole {get;set;}
        public DbSet<UserGroup> UserGroup {get;set;}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Soft Delete Filter
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

        }


    }
}
