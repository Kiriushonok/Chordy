using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToDto(Author author, int totalViews)
        {
            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                AvatarPath = author.AvatarPath,
                TotalViews = totalViews
            };
        }

        public static Author ToEntity(AuthorCreateDto authorCreateDto) {
            return new Author
            {
                Name = authorCreateDto.Name,
                AvatarPath = authorCreateDto.AvatarPath
            };
        }
    }
}
