using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface ICollectionService
    {
        Task<CollectionDto> CreateAsync(CollectionCreateDto collectionDto, CancellationToken cancellationToken = default);
        Task<CollectionDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, CollectionCreateDto collectionDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<CollectionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
