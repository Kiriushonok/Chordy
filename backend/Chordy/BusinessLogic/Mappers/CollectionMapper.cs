using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Mappers
{
    public class CollectionMapper
    {
        public static CollectionDto ToDto(Collection collection)
        {
            return new CollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
            };
        }

        public static Collection ToEntity(CollectionCreateDto collectionDto)
        {
            return new Collection
            {
                Name = collectionDto.Name
            };
        }
    }
}
