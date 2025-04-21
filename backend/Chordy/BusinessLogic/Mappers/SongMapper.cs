using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Mappers
{
    public static class SongMapper
    {
        public static SongDto ToDto(Song song)
        {
            return new SongDto
            {
                Id = song.Id,
                Name = song.Name,
                Text = song.Text,
                Views = song.Views,
                Date = song.Date,
                UserId = song.UserId,
                UserLogin = song.User.Login,
                IsPublic = song.IsPublic,
                Authors = song.SongAuthors.Select(sa => new AuthorDto
                {
                    Id = sa.Author.Id,
                    Name = sa.Author.Name
                }).ToList(),
                Collections = song.SongCollections.Select(sc => new CollectionDto
                {
                    Id = sc.Collection.Id,
                    Name = sc.Collection.Name
                }).ToList()
            };
        }

        public static Song ToEntity(SongCreateDto songCreateDto, User user, List<Author> authors, List<Collection> collections) 
        {
            return new Song
            {
                Name = songCreateDto.Name,
                Text = songCreateDto.Text,
                UserId = songCreateDto.UserId,
                User = user,
                IsPublic = songCreateDto.IsPublic,
                Date = DateTime.UtcNow,
                SongAuthors = authors.Select(a => new SongAuthor { AuthorId = a.Id }).ToList(),
                SongCollections = collections.Select(c => new SongCollection { CollectionId = c.Id }).ToList()
            };
        }
    }
}
