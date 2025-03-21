using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> CreateAsync(string name, CancellationToken cancellationToken = default);
    }
}
