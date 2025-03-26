using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class CollectionRepository(ChordyDbContext context) : ICollectionRepository
    {
        public async Task CreateAsync(Collection collection, CancellationToken cancellationToken = default)
        {
            await context.collections.AddAsync(collection, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Collection collection, CancellationToken cancellationToken = default)
        {
            context.collections.Remove(collection);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Collection>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.collections.ToListAsync(cancellationToken);
        }

        public async Task<Collection?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.collections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Collection?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await context.collections.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
        }

        public async Task UpdateAsync(Collection collection, CancellationToken cancellationToken = default)
        {
            context.collections.Update(collection);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
