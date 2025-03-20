namespace Chordy.DataAccess
{
    public interface IAuthorRepository
    {
        Task CreateAsync(Author author, CancellationToken cancellationToken = default);
    }
}
