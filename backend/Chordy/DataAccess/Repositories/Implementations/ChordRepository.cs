using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class ChordRepository(ChordyDbContext context) : IChordRepository
    {
        public async Task CreateAsync(Chord chord, CancellationToken cancellationToken = default)
        {
            await context.chords.AddAsync(chord, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Chord chord, CancellationToken cancellationToken = default)
        {
            context.chords.Remove(chord);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Chord>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.chords.ToListAsync(cancellationToken);
        }

        public async Task<Chord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.chords.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Chord?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await context.chords.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public async Task UpdateAsync(Chord chord, CancellationToken cancellationToken = default)
        {
            context.chords.Update(chord);
            await context.SaveChangesAsync();
        }
    }
}
