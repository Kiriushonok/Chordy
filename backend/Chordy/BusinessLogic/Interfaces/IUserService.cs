using Chordy.BusinessLogic.Models;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(UserRegisterDto userRegister, CancellationToken cancellationToken = default);
        Task<UserDto> GetUserByLoginAsync(string login, CancellationToken cancellationToken = default);
        Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(string login, UserRegisterDto userRegister, CancellationToken cancellationToken = default);
        Task DeleteAsync(string login, CancellationToken cancellationToken = default);
        Task<(string accessToken, string refreshToken)> LoginUserAsync(UserRegisterDto userRegisterDto, CancellationToken cancellationToken = default);
        Task<(string accessToken, string refreshToken, bool isPersistant)> RefreshTokensAsync(string? refreshToken, CancellationToken cancellationToken = default);
    }
}
