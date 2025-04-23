using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface IChordVariationRepository
    {
        Task<ChordVariation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<ChordVariation>> GetByChordIdAsync(int chordId, CancellationToken cancellationToken = default);
        Task AddAsync(ChordVariation variation, CancellationToken cancellationToken = default);
        Task UpdateAsync(ChordVariation chordVariation, CancellationToken cancellationToken = default);
        Task DeleteAsync(ChordVariation chordVariation, CancellationToken cancellationToken = default);
    }
}
