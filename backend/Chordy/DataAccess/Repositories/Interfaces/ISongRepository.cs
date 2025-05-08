using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface ISongRepository
    {
        Task CreateAsync(Song song, CancellationToken cancellationToken = default);
        Task<Song?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Song>> GetAllAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(Song song, CancellationToken cancellationToken = default);
        Task DeleteAsync(Song song, CancellationToken cancellationToken = default);

        Task<List<Song>> GetPublicByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<Song>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<Song>> GetByCollectionIdAsync(int collectionId, CancellationToken cancellationToken = default);
        Task<List<Song>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default);
    }
}
