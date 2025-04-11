using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;

namespace AuthenticationApi.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetAllAsync();
        Task<Group?> GetByIdAsync(int id);
        Task<Group> CreateAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(int id);
        Task<Group?> GetGroupWithUsersAsync(int id);
    }
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _groupRepository.GetAllAsync();
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _groupRepository.GetByIdAsync(id);
        }

        public async Task<Group> CreateAsync(Group group)
        {
            return await _groupRepository.AddAsync(group);
        }

        public async Task UpdateAsync(Group group)
        {
            await _groupRepository.UpdateAsync(group);
        }

        public async Task DeleteAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group != null)
            {
                await _groupRepository.RemoveAsync(group);
            }
        }

        public async Task<Group?> GetGroupWithUsersAsync(int id)
        {
            return await _groupRepository.GetGroupWithUsersAsync(id);
        }
    }
}