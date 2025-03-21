using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface IUserTempRepository : IRepository<TempUser>
{
    //Task<User> GetByUsernameAsync(string username);
    Task<TempUser> GetByEmailAsync(string email);

    
}


public class UserTempRepository : Repository<TempUser>, IUserTempRepository
{
    public UserTempRepository(AppDbContext context) : base(context) { }

    public async Task<TempUser> GetByEmailAsync(string email)
    {
       return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
    }
}