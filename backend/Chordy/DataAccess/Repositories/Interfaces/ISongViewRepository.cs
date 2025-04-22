namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface ISongViewRepository
    {
        Task<bool> HasUserViewedAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
        Task AddViewAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
    }
}
