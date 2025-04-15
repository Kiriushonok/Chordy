using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToDto(Author author)
        {
            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
            };
        }

        public static Author ToEntity(AuthorCreateDto authorDto) {
            return new Author
            {
                Name = authorDto.Name
            };
        }
    }
}
