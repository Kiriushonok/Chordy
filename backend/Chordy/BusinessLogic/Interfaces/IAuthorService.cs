using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> CreateAsync(string name, CancellationToken cancellationToken = default);
        Task<string> GetAuthorNameByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Author> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
