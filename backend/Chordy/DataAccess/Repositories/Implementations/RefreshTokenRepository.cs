using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class RefreshTokenRepository(ChordyDbContext context) : IRefreshTokenRepository
    {
        public async Task CreateAsync(RefreshToken refreshTokenEntity, CancellationToken cancellationToken = default)
        {
            await context.refreshTokens.AddAsync(refreshTokenEntity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await context.refreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        }

        public async Task UpdateAsync(RefreshToken refreshTokenEntity, CancellationToken cancellationToken = default)
        {
            context.refreshTokens.Update(refreshTokenEntity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteOldTokensAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var oldTokens = context.refreshTokens
                .Where(t => t.IsRevoked || t.ExpiresAt < now)
                .ToList();

            if (oldTokens.Any())
            {
                context.refreshTokens.RemoveRange(oldTokens);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
