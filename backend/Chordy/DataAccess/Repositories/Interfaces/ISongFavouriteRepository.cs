using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface ISongFavouriteRepository
    {
        Task<bool> IsFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
        Task AddFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
        Task DeleteFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
        Task<List<Song>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetFavouritesCountAsync(int songId, CancellationToken cancellationToken = default);
        Task<Dictionary<int, int>> GetFavouritesCountForSongsAsync(IEnumerable<int> songIds, CancellationToken cancellationToken = default);
    }
}
