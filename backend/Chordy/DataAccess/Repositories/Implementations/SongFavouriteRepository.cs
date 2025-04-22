using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class SongFavouriteRepository(ChordyDbContext context) : ISongFavouriteRepository
    {
        public async Task AddFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            if (!await IsFavouriteAsync(userId, songId, cancellationToken))
            {
                context.songFavourites.Add(new SongFavourite { SongId = songId, UserId = userId });
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            var favourite = await context.songFavourites.FirstOrDefaultAsync(sf => sf.UserId == userId && sf.SongId == songId, cancellationToken);
            if (favourite != null) 
            {
                context.songFavourites.Remove(favourite);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<Song>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var songIds = await context.songFavourites
                .Where(sf => sf.UserId == userId)
                .Select(sf => sf.SongId)
                .ToListAsync(cancellationToken);

            return await context.songs
                .Where(s => songIds.Contains(s.Id))
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            return await context.songFavourites.AnyAsync(sf => sf.UserId == userId && sf.SongId == songId);
        }

        public async Task<int> GetFavouritesCountAsync(int songId, CancellationToken cancellationToken = default)
        {
            return await context.songFavourites.CountAsync(sf => sf.SongId == songId, cancellationToken);
        }

        public async Task<Dictionary<int, int>> GetFavouritesCountForSongsAsync(IEnumerable<int> songIds, CancellationToken cancellationToken = default)
        {
            return await context.songFavourites
                .Where(sf => songIds.Contains(sf.SongId))
                .GroupBy(sf => sf.SongId)
                .Select(g => new { SongId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SongId, x => x.Count, cancellationToken);
        }
    }
}
