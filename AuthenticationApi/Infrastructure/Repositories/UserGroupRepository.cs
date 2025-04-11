using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface IUserGroupRepository : IRepository<UserGroup>
    {
        Task<IEnumerable<UserGroup>> GetUserGroupsByUserIdAsync(Guid userId);
    }
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<UserGroup>> GetUserGroupsByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(ug => ug.Group)
                .Where(ug => ug.UserId == userId)
                .ToListAsync();
        }
    }
    
}