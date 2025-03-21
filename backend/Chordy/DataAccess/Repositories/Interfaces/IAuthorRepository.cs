using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task CreateAsync(Author author, CancellationToken cancellationToken = default);
        Task<Author?> GetByIdAsync(int id,  CancellationToken cancellationToken = default);
        Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
        Task DeleteAsync(Author author, CancellationToken cancellationToken = default);
    }
}
