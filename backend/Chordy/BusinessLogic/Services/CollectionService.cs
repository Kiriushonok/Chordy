using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.BusinessLogic.Services
{
    public class CollectionService(ICollectionRepository collectionRepository) : ICollectionService
    {
        public async Task<Collection> CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя подборки не может быть пустым или состоять только из пробелов.", nameof(name));
            var existingCollection = await collectionRepository.GetByNameAsync(name, cancellationToken);
            if (existingCollection != null) {
                throw new DuplicationConflictException($"Подборка с названием {name} уже существует");
            }
            var collection = new Collection
            {
                Name = name,
            };
            await collectionRepository.CreateAsync(collection, cancellationToken);
            return collection;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var collection = await collectionRepository.GetByIdAsync(id, cancellationToken);
            if (collection == null) {
                throw new KeyNotFoundException($"Подборка с ID {id} не найдена");
            }
            await collectionRepository.DeleteAsync(collection, cancellationToken);
        }

        public async Task<List<Collection>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await collectionRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Collection> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var collection = await collectionRepository.GetByIdAsync(id, cancellationToken);
            if (collection == null) {
                throw new KeyNotFoundException($"Подборка с ID {id} не найдена");
            }
            return collection;
        }

        public async Task UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
        {
            var collection = await collectionRepository.GetByIdAsync(id, cancellationToken);
            if (collection == null) {
                throw new KeyNotFoundException($"Подборка с ID {id} не найдена");
            }
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя подборки не может быть пустым или состоять только из пробелов.", nameof(name));
            collection.Name = name;
            await collectionRepository.UpdateAsync(collection);
        }
    }
}
