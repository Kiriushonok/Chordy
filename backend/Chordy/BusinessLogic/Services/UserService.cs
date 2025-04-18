﻿using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Validators;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.BusinessLogic.Services
{
    public class UserService(IUserRepository userRepository, IJwtProvider jwtProvider) : IUserService
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
            await userRepository.CreateAsync(user);

            return UserMapper.ToDto(user);
        }

        public async Task DeleteAsync(string login, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByLoginAsync(login, cancellationToken)
                ?? throw new KeyNotFoundException($"Пользователь с логином '{login} не найден'");
            await userRepository.DeleteAsync(user);
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
            await userRepository.UpdateAsync(user);
        }

        public async Task<string> LoginUserAsync(UserRegisterDto userRegisterDto, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByLoginAsync(userRegisterDto.Login, cancellationToken) ?? throw new Exception("Неверный логин или пароль");

            var result = PasswordHasher.Verify(userRegisterDto.Password, user.PasswordHash);

            if (result == false) {
                throw new InvalidOperationException("Неверный логин или пароль");
            }

            var token = jwtProvider.GenerateToken(user);

            return token;
        }
    }
}
