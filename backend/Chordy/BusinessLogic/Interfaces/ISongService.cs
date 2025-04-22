using Chordy.BusinessLogic.Models;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface ISongService
    {
        Task<SongDto> CreateAsync(SongCreateDto dto, CancellationToken cancellationToken = default);
        Task<SongDto?> GetByIdAsync(int id, Guid? userId = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, SongCreateDto songCreateDto, CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetByCollectionIdAsync(int collectionId, CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default);
    }
}
