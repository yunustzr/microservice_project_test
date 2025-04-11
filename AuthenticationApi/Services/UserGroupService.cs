using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure.Repositories;

namespace AuthenticationApi.Services
{
    public interface IUserGroupService
    {
        Task<IEnumerable<UserGroup>> GetAllAsync();
        Task<UserGroup?> GetByIdAsync(int id);
        Task<UserGroup> CreateAsync(UserGroup userGroup);
        Task<IEnumerable<UserGroup>> GetUserGroupsByUserIdAsync(Guid userId);
        Task DeleteAsync(int id);
    }
    public class UserGroupService : IUserGroupService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        public UserGroupService(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<IEnumerable<UserGroup>> GetAllAsync()
        {
            return await _userGroupRepository.GetAllAsync();
        }

        public async Task<UserGroup?> GetByIdAsync(int id)
        {
            return await _userGroupRepository.GetByIdAsync(id);
        }

        public async Task<UserGroup> CreateAsync(UserGroup userGroup)
        {
            return await _userGroupRepository.AddAsync(userGroup);
        }

        public async Task<IEnumerable<UserGroup>> GetUserGroupsByUserIdAsync(Guid userId)
        {
            return await _userGroupRepository.GetUserGroupsByUserIdAsync(userId);
        }

        public async Task DeleteAsync(int id)
        {
            var userGroup = await _userGroupRepository.GetByIdAsync(id);
            if (userGroup != null)
            {
                await _userGroupRepository.RemoveAsync(userGroup);
            }
        }
    }
}