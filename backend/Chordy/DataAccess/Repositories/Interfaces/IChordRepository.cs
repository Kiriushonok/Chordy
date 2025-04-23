using Chordy.DataAccess.Entities;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface IChordRepository
    {
        Task CreateAsync(Chord chord, CancellationToken cancellationToken = default);
        Task DeleteAsync(Chord chord, CancellationToken cancellationToken = default);
        Task<Chord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Chord chord, CancellationToken cancellationToken = default);
        Task<List<Chord>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Chord?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
