using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Chordy.BusinessLogic.Validators;
using Chordy.BusinessLogic.Utils;
namespace Chordy.BusinessLogic.Services
{
    internal class AuthorService(IAuthorRepository authorRepository, ISongRepository songRepository) : IAuthorService
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

        public async Task<List<PopularAuthorDto>> GetTopAuthorsByViewsAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            var authors = await authorRepository.GetAllAsync(cancellationToken);
            var result = new List<PopularAuthorDto>();

            foreach (var author in authors)
            {
                var songs = await songRepository.GetByAuthorIdAsync(author.Id, cancellationToken);
                int totalViews = songs.Sum(s => s.Views);

                result.Add(new PopularAuthorDto
                {
                    Id = author.Id,
                    Name = author.Name,
                    AvatarPath = author.AvatarPath,
                    TotalViews = totalViews
                });
            }

            return result.OrderByDescending(a => a.TotalViews).Take(count).ToList();
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

        public async Task<PagedResult<PopularAuthorDto>> GetPagedAuthorsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var authors = await authorRepository.GetAllAsync(cancellationToken);
            var result = new List<PopularAuthorDto>();
            foreach (var author in authors)
            {
                var songs = await songRepository.GetByAuthorIdAsync(author.Id, cancellationToken);
                int totalViews = songs.Sum(s => s.Views);
                result.Add(new PopularAuthorDto
                {
                    Id = author.Id,
                    Name = author.Name,
                    AvatarPath = author.AvatarPath,
                    TotalViews = totalViews
                });
            }
            var paged = result.OrderByDescending(a => a.TotalViews)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

            return new PagedResult<PopularAuthorDto>
            {
                Items = paged,
                TotalCount = result.Count
            };
        }
    }
}