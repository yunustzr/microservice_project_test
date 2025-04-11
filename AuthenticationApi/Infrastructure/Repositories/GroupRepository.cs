using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<Group?> GetGroupWithUsersAsync(int groupId);
    }
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(AppDbContext context) : base(context) { }

        public async Task<Group?> GetGroupWithUsersAsync(int groupId)
        {
            return await _dbSet
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }
    }

}