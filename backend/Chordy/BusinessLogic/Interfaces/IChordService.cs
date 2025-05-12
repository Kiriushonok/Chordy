using Chordy.BusinessLogic.Models;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface IChordService
    {
        Task<ChordDto> CreateAsync(ChordCreateDto chordCreateDto, CancellationToken cancellationToken = default);
        Task<ChordDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, ChordCreateDto chordCreateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<ChordDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ChordDto> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<List<ChordDto>> SearchChordsByNameAsync(string query, CancellationToken cancellationToken = default);
    }
}
