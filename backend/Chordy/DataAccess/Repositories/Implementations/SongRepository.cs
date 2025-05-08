using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class SongRepository(ChordyDbContext context) : ISongRepository
    {
        public async Task CreateAsync(Song song, CancellationToken cancellationToken = default)
        {
            await context.songs.AddAsync(song, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Song song, CancellationToken cancellationToken = default)
        {
            context.songs.Remove(song);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Song>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.songs
                .Where(s => s.IsPublic)
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Song>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default)
        {
            return await context.songs
                .Where(s => s.IsPublic && s.SongAuthors.Any(sa => sa.AuthorId == authorId))
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Song>> GetByCollectionIdAsync(int collectionId, CancellationToken cancellationToken = default)
        {
            return await context.songs
                .Where(s => s.IsPublic && s.SongCollections.Any(sc => sc.CollectionId == collectionId))
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .ToListAsync(cancellationToken);
        }

        public async Task<Song?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.songs
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<Song>> GetPublicByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.songs
                .Where(s => s.UserId == userId && s.IsPublic)
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Song>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.songs
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
                .Include(s => s.SongCollections).ThenInclude(sc => sc.Collection)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Song song, CancellationToken cancellationToken = default)
        {
            context.songs.Update(song);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
