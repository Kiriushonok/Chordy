﻿using Chordy.BusinessLogic.Exceptions;
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

            Author author = AuthorMapper.ToEntity(authorCreateDto);
            await authorRepository.CreateAsync(author, cancellationToken);

            return AuthorMapper.ToDto(author);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
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
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            return AuthorMapper.ToDto(author);
        }

        public async Task<AuthorDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByNameAsync(name, cancellationToken);
            if (author == null)
            {
                throw new KeyNotFoundException($"Автор {name} не найден");
            }
            return AuthorMapper.ToDto(author);
        }

        public async Task UpdateAsync(int id, AuthorCreateDto authorDto, CancellationToken cancellationToken = default)
        {
            AuthorValidator.Validate(authorDto);
            var author = await authorRepository.GetByIdAsync(id, cancellationToken);

            if (author == null)
            {
                throw new KeyNotFoundException($"Автор с ID {id} не найден");
            }

            author.Name = authorDto.Name;
            await authorRepository.UpdateAsync(author, cancellationToken);
        }
    }
}