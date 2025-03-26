using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface ICollectionService
    {
        Task<Collection> CreateAsync(string name, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<Collection> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Collection>> GetAllAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, CancellationToken cancellationToken = default);
    }
}
