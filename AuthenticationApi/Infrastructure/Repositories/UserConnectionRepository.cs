using AuthenticationApi.Domain.Models.ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface IUserConnectionRepository : IRepository<UserConnection>
    {
        Task<UserConnection> GetByConnectionIdAsync(string connectionId);
        Task<List<UserConnection>> GetActiveConnectionsByUserIdAsync(Guid userId);
        Task<bool> HasActiveConnectionsAsync(Guid userId);
        Task<int> RemoveInactiveConnectionsAsync(TimeSpan maxInactivityDuration);
        Task<List<UserConnection>> GetAllActiveConnectionsAsync();
    }

    public class UserConnectionRepository : Repository<UserConnection>, IUserConnectionRepository
    {
        public UserConnectionRepository(AppDbContext context) : base(context) { }

        public async Task<UserConnection> GetByConnectionIdAsync(string connectionId)
        {
            return await _dbSet
                .Include(uc => uc.User)
                .FirstOrDefaultAsync(uc => uc.ConnectionId == connectionId);
        }

        public async Task<List<UserConnection>> GetActiveConnectionsByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(uc => uc.UserId == userId && uc.DisconnectedAt == null)
                .ToListAsync();
        }

        public async Task<bool> HasActiveConnectionsAsync(Guid userId)
        {
            return await _dbSet
                .AnyAsync(uc => uc.UserId == userId && uc.DisconnectedAt == null);
        }

        public async Task<int> RemoveInactiveConnectionsAsync(TimeSpan maxInactivityDuration)
        {
            var cutoff = DateTime.UtcNow.Subtract(maxInactivityDuration);
            var inactiveConnections = await _dbSet
                .Where(uc => uc.DisconnectedAt < cutoff)
                .ToListAsync();

            _dbSet.RemoveRange(inactiveConnections);
            return await _context.SaveChangesAsync();
        }

        public override async Task<UserConnection> AddAsync(UserConnection entity)
        {
            if (await _dbSet.AnyAsync(uc => uc.ConnectionId == entity.ConnectionId))
            {
                throw new InvalidOperationException("Connection ID already exists");
            }

            return await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(UserConnection entity)
        {
            entity.LastModified = DateTime.UtcNow;
            await base.UpdateAsync(entity);
        }

        public async Task<List<UserConnection>> GetAllActiveConnectionsAsync()
        {
            return await _dbSet
                .Where(uc => uc.DisconnectedAt == null)
                .ToListAsync();
        }

    }

}
