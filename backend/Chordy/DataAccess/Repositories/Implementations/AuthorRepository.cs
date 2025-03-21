using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.DataAccess.Repositories.Implementations
{
    internal class AuthorRepository(AppContext context) : IAuthorRepository
    {
        public async Task CreateAsync(Author author, CancellationToken cancellationToken = default)
        {
            await context.authors.AddAsync(author, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
