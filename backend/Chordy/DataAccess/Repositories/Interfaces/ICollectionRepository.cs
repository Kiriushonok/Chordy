using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface ICollectionRepository
    {
        Task CreateAsync(Collection collection, CancellationToken cancellationToken = default);
        Task DeleteAsync(Collection collection, CancellationToken cancellationToken = default);
        Task<Collection?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Collection collection, CancellationToken cancellationToken = default);
        Task<List<Collection>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Collection?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
