using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorDto> CreateAsync(AuthorCreateDto authorCreateDto, CancellationToken cancellationToken = default);
        Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AuthorDto> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, AuthorCreateDto authorDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<PopularAuthorDto>> GetTopAuthorsByViewsAsync(int count = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<PopularAuthorDto>> GetPagedAuthorsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<List<AuthorDto>> SearchAuthorsByNameAsync(string query, CancellationToken cancellationToken = default);
    }
}
