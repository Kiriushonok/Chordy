using Chordy.BusinessLogic.Interfaces;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.BusinessLogic.Services
{
    internal class AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger) : IAuthorService
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
                logger.LogWarning("Автор с ID {id} не найден", id);
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            await authorRepository.DeleteAsync(author, cancellationToken);
        }

        public async Task<string> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                logger.LogWarning("Автор с ID {id} не найден", id);
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            return author.name;
        }

        public async Task UpdateAsync(int id, string newName, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                logger.LogWarning("Автор с ID {id} не найден", id);
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            author.name = newName;
            await authorRepository.UpdateAsync(author, cancellationToken);
        }
    }
}
