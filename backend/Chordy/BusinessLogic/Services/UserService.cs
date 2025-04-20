using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Utils;
using Chordy.BusinessLogic.Validators;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using System.Security;

namespace Chordy.BusinessLogic.Services
{
    public class UserService(IUserRepository userRepository, IJwtProvider jwtProvider, IRefreshTokenRepository refreshTokenRepository) : IUserService
    {
        public async Task<UserDto> CreateAsync(UserRegisterDto userRegister, CancellationToken cancellationToken = default)
        {
            UserValidator.Validate(userRegister);
            var existingUser = await userRepository.GetByLoginAsync(userRegister.Login, cancellationToken);
            if (existingUser != null)
            {
                throw new DuplicationConflictException($"Пользователь с логином '{userRegister.Login}' уже существует");
            }
            User user = UserMapper.ToEntity(userRegister);
            user.Id = Guid.NewGuid();
            await userRepository.CreateAsync(user, cancellationToken);

            return UserMapper.ToDto(user);
        }

        public async Task DeleteAsync(string login, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByLoginAsync(login, cancellationToken)
                ?? throw new KeyNotFoundException($"Пользователь с логином '{login} не найден'");
            await userRepository.DeleteAsync(user, cancellationToken);
        }

        public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await userRepository.GetAllAsync(cancellationToken);
            return users.Select(UserMapper.ToDto).ToList();
        }

        public async Task<UserDto> GetUserByLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByLoginAsync(login, cancellationToken)
                ?? throw new KeyNotFoundException($"Пользователь с логином '{login}' не найден");
            return UserMapper.ToDto(user);
        }

        public async Task UpdateAsync(string login, UserRegisterDto userRegister, CancellationToken cancellationToken = default)
        {
            UserValidator.Validate(userRegister);
            var user = await userRepository.GetByLoginAsync(login, cancellationToken)
                ?? throw new KeyNotFoundException($"Пользователь с логином '{login}' не найден");
            var existingUser = await userRepository.GetByLoginAsync(login, cancellationToken);
            if (existingUser != null && existingUser.Login != login) 
            {
                throw new DuplicationConflictException($"Пользователь с логином {login} уже существует");
            }
            user.Login = login;
            user.PasswordHash = PasswordHasher.Generate(userRegister.Password);
            await userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task<(string accessToken, string refreshToken)> LoginUserAsync(UserRegisterDto userRegisterDto, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByLoginAsync(userRegisterDto.Login, cancellationToken) ?? throw new UnauthorizedAccessException("Неверный логин или пароль");

            var result = PasswordHasher.Verify(userRegisterDto.Password, user.PasswordHash);

            if (result == false) {
                throw new InvalidOperationException("Неверный логин или пароль");
            }

            var accessToken = jwtProvider.GenerateToken(user);

            var refreshToken = TokenHelper.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = userRegisterDto.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(2),
                IsRevoked = false,
            };
            await refreshTokenRepository.CreateAsync(refreshTokenEntity, cancellationToken);

            return (accessToken, refreshToken);
        }

        public async Task<(string accessToken, string refreshToken, bool isPersistant)> RefreshTokensAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new SecurityException("Refresh token отсутствует");

            var tokenEntity = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            if (tokenEntity == null || tokenEntity.IsRevoked == true || tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new SecurityException("Refresh token недействителен");
            }

            var user = await userRepository.GetByIdAsync(tokenEntity.UserId, cancellationToken) ?? throw new KeyNotFoundException($"Пользователь не найден'");
            tokenEntity.IsRevoked = true;
            await refreshTokenRepository.UpdateAsync(tokenEntity, cancellationToken);

            var newAccessToken = jwtProvider.GenerateToken(user);
            var newRefreshToken = TokenHelper.GenerateRefreshToken();
            bool isPersistent = (tokenEntity.ExpiresAt - DateTime.UtcNow).TotalDays > 1;
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = isPersistent ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(1),
                IsRevoked = false
            };

            await refreshTokenRepository.CreateAsync(newRefreshTokenEntity, cancellationToken);

            return (newAccessToken, newRefreshToken, isPersistent);
        }
    }
}
