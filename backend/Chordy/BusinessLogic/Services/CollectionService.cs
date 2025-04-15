using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Chordy.BusinessLogic.Validators;
namespace Chordy.BusinessLogic.Services
{
    public class CollectionService(ICollectionRepository collectionRepository) : ICollectionService
    {
        public async Task<CollectionDto> CreateAsync(CollectionCreateDto collectionCreateDto, CancellationToken cancellationToken = default)
        {
            CollectionValidator.Validate(collectionCreateDto);
            var existingCollection = await collectionRepository.GetByNameAsync(collectionCreateDto.Name, cancellationToken);
            if (existingCollection != null) 
            {
                throw new DuplicationConflictException($"Подборка с названием {collectionCreateDto.Name} уже существует");
            }
            var collection = CollectionMapper.ToEntity(collectionCreateDto);
            await collectionRepository.CreateAsync(collection, cancellationToken);
            return CollectionMapper.ToDto(collection);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var collection = await collectionRepository.GetByIdAsync(id, cancellationToken);
            if (collection == null) {
                throw new KeyNotFoundException($"Подборка с ID {id} не найдена");
            }
            await collectionRepository.DeleteAsync(collection, cancellationToken);
        }

        public async Task<List<CollectionDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var collections = await collectionRepository.GetAllAsync(cancellationToken);
            return collections.Select(CollectionMapper.ToDto).ToList();
        }

        public async Task<CollectionDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var collection = await collectionRepository.GetByIdAsync(id, cancellationToken);
            if (collection == null) {
                throw new KeyNotFoundException($"Подборка с ID {id} не найдена");
            }
            return CollectionMapper.ToDto(collection);
        }

        public async Task UpdateAsync(int id, CollectionCreateDto collectionCreateDto, CancellationToken cancellationToken = default)
        {
            CollectionValidator.Validate(collectionCreateDto);
            var collection = await collectionRepository.GetByIdAsync(id, cancellationToken);
            if (collection == null) {
                throw new KeyNotFoundException($"Подборка с ID {id} не найдена");
            }
            collection.Name = collectionCreateDto.Name;
            await collectionRepository.UpdateAsync(collection);
        }
    }
}
