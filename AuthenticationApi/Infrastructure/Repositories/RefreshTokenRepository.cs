using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface IRefreshTokenRepository : IRepository<RefreshTokens>
{
    Task InvalidateAllAsync(Guid userId);
    Task CreateAsync(RefreshTokens token);
}
public class RefreshTokenRepository : Repository<RefreshTokens>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task InvalidateAllAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.Revoked == null)
            .ToListAsync();

        foreach (var tk in tokens)
            tk.Revoked = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(RefreshTokens token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

}


