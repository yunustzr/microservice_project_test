using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;


public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetUserWithRefreshTokensAsync(Guid userId);
    Task<User> GetUserWithRolesAsync(Guid userId);
    Task AssignRoleToUserAsync(Guid userId, int roleId);
    Task<User> GetUserByRefreshTokenAsync(string refreshToken);
    Task DeleteAsync(Guid id);
    Task<bool> ChangePasswordAsync(Guid userId, string newPassword);
}


public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
    }

    public async Task<User> GetUserWithRefreshTokensAsync(Guid userId)
    {
        return await _dbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User> GetUserWithRolesAsync(Guid userId)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AssignRoleToUserAsync(Guid userId, int roleId)
    {
        var user = await _dbSet.FindAsync(userId);
        var userRole = new UserRoles { UserId = userId, RoleId = roleId, AssignedAt = DateTime.UtcNow };
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
    }
    public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _dbSet.FindAsync(id);
        if (user != null)
        {
            await RemoveAsync(user);
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string newPassword)
    {
        var user = _dbSet.FindAsync(userId);
        if(user == null)
        {
            return false;
        }
        user.Result.PasswordHash = newPassword; // Assuming PasswordHash is a string, adjust as necessary
        _context.Entry(user.Result).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }
}