using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Chordy.BusinessLogic.Validators;
namespace Chordy.BusinessLogic.Services
{
    internal class AuthorService(IAuthorRepository authorRepository) : IAuthorService
    {
        public async Task<AuthorDto> CreateAsync(AuthorCreateDto authorCreateDto, CancellationToken cancellationToken = default)
        {
            AuthorValidator.Validate(authorCreateDto);
            var existingAuthor = await authorRepository.GetByNameAsync(authorCreateDto.Name, cancellationToken);
            if (existingAuthor != null)
            {
                throw new DuplicationConflictException($"Автор с именем '{authorCreateDto.Name}' уже существует.");
            }
            if (authorCreateDto.Avatar != null)
            {
                ImageValidator.ValidateImage(authorCreateDto.Avatar);
            }
            authorCreateDto.AvatarPath = await FileHelper.SaveAvatarAsync(authorCreateDto.Avatar, authorCreateDto.Name, cancellationToken: cancellationToken);
            Author author = AuthorMapper.ToEntity(authorCreateDto);
            await authorRepository.CreateAsync(author, cancellationToken);

            return AuthorMapper.ToDto(author);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Автор с ID {id} не найден");

            if (!string.IsNullOrEmpty(author.AvatarPath))
            {
                FileHelper.DeleteAvatar(author.AvatarPath);
            }

            await authorRepository.DeleteAsync(author, cancellationToken);
        }

        public async Task<List<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var authors = await authorRepository.GetAllAsync(cancellationToken);
            return authors.Select(AuthorMapper.ToDto).ToList();
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Автор с ID {id} не найден");

            return AuthorMapper.ToDto(author);
        }

        public async Task<AuthorDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByNameAsync(name, cancellationToken) ?? throw new KeyNotFoundException($"Автор {name} не найден");

            return AuthorMapper.ToDto(author);
        }

        public async Task UpdateAsync(int id, AuthorCreateDto authorCreateDto, CancellationToken cancellationToken = default)
        {
            AuthorValidator.Validate(authorCreateDto);
            var author = await authorRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Автор с ID {id} не найден");

            var existingAuthor = await authorRepository.GetByNameAsync(authorCreateDto.Name, cancellationToken);
            if (existingAuthor != null && existingAuthor.Id != id)
            {
                throw new DuplicationConflictException($"Автор с именем '{authorCreateDto.Name}' уже существует.");
            }

            if (!string.IsNullOrEmpty(author.AvatarPath))
            {
                FileHelper.DeleteAvatar(author.AvatarPath);
            }
            if (authorCreateDto.Avatar != null)
            {
                ImageValidator.ValidateImage(authorCreateDto.Avatar);
            }
            authorCreateDto.AvatarPath = await FileHelper.SaveAvatarAsync(authorCreateDto.Avatar, authorCreateDto.Name, cancellationToken: cancellationToken);
            author.Name = authorCreateDto.Name;
            author.AvatarPath = authorCreateDto.AvatarPath;
            await authorRepository.UpdateAsync(author, cancellationToken);
        }
    }
}