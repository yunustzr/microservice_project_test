using System.Data.Common;
using System.Diagnostics;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuthenticationApi.Infrastructure.Interceptors
{
    public class SelectAuditInterceptor : DbCommandInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AuditLogIgnoreFilter _auditLogIgnoreFilter = new();

        public SelectAuditInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            await LogQueryAsync(command, eventData, cancellationToken);
            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        private async Task LogQueryAsync(DbCommand command, CommandEventData eventData, CancellationToken cancellationToken)
        {
            try
            {
                if (ShouldLogCommand(command))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var auditLog = new AuditLog
                    {
                        ServiceName = "AuthenticationApi",
                        Action = "SELECT",
                        EntityName = "Query",
                        EntityId = "N/A",
                        ChangedBy = GetCurrentUser(),
                        ChangedAt = DateTime.UtcNow,
                        NewValues = Truncate(command.CommandText, 4000)
                    };

                    await dbContext.AuditLog.AddAsync(auditLog, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Audit log error: {ex.Message}");
            }
        }

        private bool ShouldLogCommand(DbCommand command)
        {
            return command.CommandText.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) &&
                   !_auditLogIgnoreFilter.IsIgnored(command.CommandText);
        }

        private static string Truncate(string value, int maxLength) =>
            value.Length <= maxLength ? value : value[..maxLength];

        private string GetCurrentUser()
        {
            return _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.User?.Identity?.Name
                   ?? "System";
        }
    }

    public class AuditLogIgnoreFilter
    {
        private static readonly string[] _ignoredCommands =
        {
            "SELECT * FROM AuditLogs",
            "SELECT COUNT(*) FROM"
        };

        public bool IsIgnored(string commandText) =>
            _ignoredCommands.Any(ic => commandText.Contains(ic, StringComparison.OrdinalIgnoreCase));
    }
}