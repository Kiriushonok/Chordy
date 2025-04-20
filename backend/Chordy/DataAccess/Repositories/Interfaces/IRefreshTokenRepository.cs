using System.Threading;

namespace Chordy.DataAccess.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshTokenEntity, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task UpdateAsync(RefreshToken refreshTokenEntity, CancellationToken cancellationToken = default);
        Task DeleteOldTokensAsync(CancellationToken cancellationToken = default);
    }
}
