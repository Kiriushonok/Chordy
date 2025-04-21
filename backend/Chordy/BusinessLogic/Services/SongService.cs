using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.BusinessLogic.Services
{
    public class SongService(
        ISongRepository songRepository,
        IUserRepository userRepository,
        IAuthorRepository authorRepository,
        ICollectionRepository collectionRepository
        ) : ISongService
    {
        public async Task<SongDto> CreateAsync(SongCreateDto dto, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByIdAsync(dto.UserId, cancellationToken)
                ?? throw new KeyNotFoundException("Пользователь не найден");

            var authors = (await authorRepository.GetAllAsync(cancellationToken))
                .Where(a => dto.AuthorIds.Contains(a.Id)).ToList();

            var collections = (await collectionRepository.GetAllAsync(cancellationToken))
                .Where(c => dto.CollectionIds.Contains(c.Id)).ToList();

            var song = SongMapper.ToEntity(dto, user, authors, collections);

            await songRepository.CreateAsync(song, cancellationToken);

            var createdSong = await songRepository.GetByIdAsync(song.Id, cancellationToken);
            return createdSong == null ? throw new Exception("Ошибка создания песни") : SongMapper.ToDto(createdSong);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException("Песня не найдена");
            await songRepository.DeleteAsync(song, cancellationToken);
        }

        public async Task<List<SongDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var songs = await songRepository.GetAllAsync(cancellationToken);
            return songs.Select(SongMapper.ToDto).ToList();
        }

        public async Task<List<SongDto>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default)
        {
            var songs = await songRepository.GetByAuthorIdAsync(authorId, cancellationToken);
            return songs.Select(SongMapper.ToDto).ToList();
        }

        public async Task<List<SongDto>> GetByCollectionIdAsync(int collectionId, CancellationToken cancellationToken = default)
        {
            var songs = await songRepository.GetByCollectionIdAsync(collectionId, cancellationToken);
            return songs.Select(SongMapper.ToDto).ToList();
        }

        public async Task<SongDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(id, cancellationToken);
            return song == null ? null : SongMapper.ToDto(song);
        }

        public async Task<List<SongDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var songs = await songRepository.GetByUserIdAsync(userId, cancellationToken);
            return songs.Select(SongMapper.ToDto).ToList();
        }

        public async Task UpdateAsync(int id, SongCreateDto songCreateDto, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException("Песня не найдена");

            song.Name = songCreateDto.Name;
            song.Text = songCreateDto.Text;
            song.IsPublic = songCreateDto.IsPublic;

            // Обновление авторов
            song.SongAuthors.Clear();
            var authors = (await authorRepository.GetAllAsync(cancellationToken))
                .Where(a => songCreateDto.AuthorIds.Contains(a.Id)).ToList();
            foreach (var author in authors)
            {
                song.SongAuthors.Add(new SongAuthor { SongId = song.Id, AuthorId = author.Id });
            }

            // Обновление коллекций
            song.SongCollections.Clear();
            var collections = (await collectionRepository.GetAllAsync(cancellationToken))
                .Where(c => songCreateDto.CollectionIds.Contains(c.Id)).ToList();
            foreach (var collection in collections)
            {
                song.SongCollections.Add(new SongCollection { SongId = song.Id, CollectionId = collection.Id });
            }

            await songRepository.UpdateAsync(song, cancellationToken);
        }
    }
}
