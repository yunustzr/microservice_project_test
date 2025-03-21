using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface ILoginLogService
{
    Task LogAsync(LoginLog loginLog);
    Task UpdateLogoutTimeForUserAsync(string username, DateTime logoutTime);
}
public class LoginLogService : ILoginLogService
{
    private readonly ILoginLogRepository _loginLogRepository;
    public LoginLogService(ILoginLogRepository loginLogRepository)
    {
        _loginLogRepository = loginLogRepository;
    }
    
    public async Task LogAsync(LoginLog loginLog)
    {
        await _loginLogRepository.AddAsync(loginLog);
    }

    public async Task UpdateLogoutTimeForUserAsync(string username, DateTime logoutTime)
    {
        await _loginLogRepository.UpdateLogoutTimeForUserAsync(username,logoutTime);
    }
}
