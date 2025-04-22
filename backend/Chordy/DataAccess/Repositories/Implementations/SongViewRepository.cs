using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class SongViewRepository(ChordyDbContext context) : ISongViewRepository
    {
        public async Task AddViewAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            context.songViews.Add(new SongViews { UserId = userId, SongId = songId });
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> HasUserViewedAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            return await context.songViews.AnyAsync(sw => sw.UserId == userId && sw.SongId == songId, cancellationToken);
        }
    }
}
