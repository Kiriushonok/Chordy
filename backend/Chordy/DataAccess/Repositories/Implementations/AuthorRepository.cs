using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    internal class AuthorRepository(ChordyDbContext context) : IAuthorRepository
    {
        public async Task CreateAsync(Author author, CancellationToken cancellationToken = default)
        {
            await context.authors.AddAsync(author, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Author author, CancellationToken cancellationToken = default)
        {
            context.authors.Remove(author);
            await context.SaveChangesAsync(cancellationToken);

        }

        public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.authors.ToListAsync(cancellationToken);
        }

        public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.authors.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await context.authors.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
        }

        public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
        {
            context.authors.Update(author);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}