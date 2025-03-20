namespace Chordy.BusinessLogic
{
    public interface IAuthorService
    {
        Task CreateAsync(string name, CancellationToken cancellationToken = default);
    }
}
