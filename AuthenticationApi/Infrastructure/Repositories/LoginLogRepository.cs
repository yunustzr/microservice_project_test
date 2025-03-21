using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface ILoginLogRepository : IRepository<LoginLog>
{
    // Şu an ekstra metot eklemeye gerek yok, IRepository yapınız yeterli olacaktır.
    Task UpdateLogoutTimeForUserAsync(string username, DateTime logoutTime);
}

public class LoginLogRepository : Repository<LoginLog>, ILoginLogRepository
{
    public LoginLogRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task UpdateLogoutTimeForUserAsync(string username, DateTime logoutTime)
    {
        // En son giriş kaydını (LogoutTime henüz güncellenmemiş) buluyoruz
        var loginLog = await _context.LoginLog
            .Where(l => l.Username == username && l.LogoutTime == null && l.IsSuccessful)
            .OrderByDescending(l => l.LoginTime)
            .FirstOrDefaultAsync();
            
        if (loginLog != null)
        {
            loginLog.LogoutTime = logoutTime;
            _context.LoginLog.Update(loginLog);
            await _context.SaveChangesAsync();
        }
    }
}
