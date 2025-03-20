
namespace Chordy.DataAccess
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
