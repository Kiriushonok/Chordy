using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class ChordVariationRepository(ChordyDbContext context) : IChordVariationRepository
    {
        public async Task AddAsync(ChordVariation variation, CancellationToken cancellationToken = default)
        {
            await context.chordVariations.AddAsync(variation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(ChordVariation chordVariation, CancellationToken cancellationToken = default)
        {
            context.chordVariations.Remove(chordVariation);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ChordVariation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.chordVariations
                .Include(x => x.Chord)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ChordVariation>> GetByChordIdAsync(int chordId, CancellationToken cancellationToken = default)
        {
            return await context.chordVariations
                .Where(x => x.ChordId == chordId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ChordVariation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.chordVariations
                .Include(x => x.Chord)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(ChordVariation chordVariation, CancellationToken cancellationToken = default)
        {
            context.chordVariations.Update(chordVariation);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
