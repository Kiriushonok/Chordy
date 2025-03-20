
using Chordy.DataAccess;

namespace Chordy.BusinessLogic
{
    internal class AuthorService(IAuthorRepository authorRepository) : IAuthorService
    {
        public async Task CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            var author = new Author
            {
                name = name,
            };
            await authorRepository.CreateAsync(author, cancellationToken);
        }
    }
}
