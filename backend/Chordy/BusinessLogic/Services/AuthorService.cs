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

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new Exception("Author not found");
            }

            await authorRepository.DeleteAsync(author, cancellationToken);
        }

        public async Task<string> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null) {
                throw new Exception("Author not found");
            }

            return author.name;
        }

        public async Task UpdateAsync(int id, string newName, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new Exception("Author not found");
            }

            author.name = newName;
            await authorRepository.UpdateAsync(author, cancellationToken);
        }
    }
}
