using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;

namespace AuthenticationApi.Services
{
    public interface IGroupRoleService
    {
        Task<IEnumerable<GroupRole>> GetAllAsync();
        Task<GroupRole?> GetByIdAsync(int id);
        Task<GroupRole> CreateAsync(GroupRole groupRole);
        Task<IEnumerable<GroupRole>> GetRolesByGroupIdAsync(int groupId);
        Task DeleteAsync(int id);
    }
    public class GroupRoleService : IGroupRoleService
    {
        private readonly IGroupRoleRepository _groupRoleRepository;
        public GroupRoleService(IGroupRoleRepository groupRoleRepository)
        {
            _groupRoleRepository = groupRoleRepository;
        }

        public async Task<IEnumerable<GroupRole>> GetAllAsync()
        {
            return await _groupRoleRepository.GetAllAsync();
        }

        public async Task<GroupRole?> GetByIdAsync(int id)
        {
            return await _groupRoleRepository.GetByIdAsync(id);
        }

        public async Task<GroupRole> CreateAsync(GroupRole groupRole)
        {
            return await _groupRoleRepository.AddAsync(groupRole);
        }

        public async Task<IEnumerable<GroupRole>> GetRolesByGroupIdAsync(int groupId)
        {
            return await _groupRoleRepository.GetRolesByGroupIdAsync(groupId);
        }

        public async Task DeleteAsync(int id)
        {
            var groupRole = await _groupRoleRepository.GetByIdAsync(id);
            if (groupRole != null)
            {
                await _groupRoleRepository.RemoveAsync(groupRole);
            }
        }
    }
}