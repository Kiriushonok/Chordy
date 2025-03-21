using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task CreateAsync(Author author, CancellationToken cancellationToken = default);
    }
}
