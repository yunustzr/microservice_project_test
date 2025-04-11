using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface IGroupRoleRepository : IRepository<GroupRole>
    {
        Task<IEnumerable<GroupRole>> GetRolesByGroupIdAsync(int groupId);
    }
    public class GroupRoleRepository : Repository<GroupRole>, IGroupRoleRepository
    {
        public GroupRoleRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<GroupRole>> GetRolesByGroupIdAsync(int groupId)
        {
            return await _dbSet
                .Include(gr => gr.Role)
                .Where(gr => gr.GroupId == groupId)
                .ToListAsync();
        }
    }

}