using Chordy.BusinessLogic.Interfaces;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.BusinessLogic.Services
{
    internal class AuthorService(IAuthorRepository authorRepository) : IAuthorService
    {
        public async Task<Author> CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            var author = new Author
            {
                name = name,
            };
            await authorRepository.CreateAsync(author, cancellationToken);
            return author;
        }
    }
}
