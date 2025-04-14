using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using System.Xml.Linq;

namespace Chordy.BusinessLogic.Services
{
    internal class AuthorService(IAuthorRepository authorRepository) : IAuthorService
    {
        public async Task<Author> CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя автора не может быть пустым или состоять только из пробелов.");
            var existingAuthor = await authorRepository.GetByNameAsync(name, cancellationToken);
            if (existingAuthor != null)
            {
                throw new DuplicationConflictException($"Автор с именем '{name}' уже существует.");
            }

            var author = new Author
            {
                Name = name,
            };
            await authorRepository.CreateAsync(author, cancellationToken);
            return author;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            await authorRepository.DeleteAsync(author, cancellationToken);
        }

        public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await authorRepository.GetAllAsync(cancellationToken);
        }

        public async Task<string> GetAuthorNameByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            return author.Name;
        }

        public async Task<Author> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByNameAsync(name, cancellationToken);
            if (author == null)
            {
                throw new KeyNotFoundException($"Автор {name} не найден");
            }
            return author;
        }

        public async Task UpdateAsync(int id, string newName, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Имя автора не может быть пустым или состоять только из пробелов.");

            author.Name = newName;
            await authorRepository.UpdateAsync(author, cancellationToken);
        }
    }
}