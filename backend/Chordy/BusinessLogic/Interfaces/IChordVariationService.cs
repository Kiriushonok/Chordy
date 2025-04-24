namespace Chordy.BusinessLogic.Interfaces
{
    using Chordy.BusinessLogic.Models;
    public interface IChordVariationService
    {
        Task<List<ChordVariationDto>> GetByChordIdAsync(int chordId, CancellationToken cancellationToken = default);
        Task<ChordVariationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ChordVariationDto> AddAsync(ChordVariationCreateDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<List<ChordVariationDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, ChordVariationCreateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
