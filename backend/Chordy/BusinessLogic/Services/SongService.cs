using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Validators;
using Chordy.DataAccess;
using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Implementations;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.BusinessLogic.Services
{
    public class SongService(
        ISongRepository songRepository,
        IUserRepository userRepository,
        IAuthorRepository authorRepository,
        ICollectionRepository collectionRepository,
        ISongViewRepository songViewRepository,
        ISongFavouriteRepository songFavouriteRepository,
        ChordyDbContext context
        ) : ISongService
    {
        public async Task AddToFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(songId, cancellationToken)
                ?? throw new KeyNotFoundException($"Песня с Id {songId} не найдена");
            await songFavouriteRepository.AddFavouriteAsync(userId, songId, cancellationToken);
        }

        public async Task<SongDto> CreateAsync(SongCreateDto dto, CancellationToken cancellationToken = default)
        {
            SongValidator.Validate( dto );

            var user = await userRepository.GetByIdAsync(dto.UserId, cancellationToken)
                ?? throw new KeyNotFoundException("Пользователь не найден");

            var authors = (await authorRepository.GetAllAsync(cancellationToken))
                .Where(a => dto.AuthorIds.Contains(a.Id)).ToList();

            var collections = (await collectionRepository.GetAllAsync(cancellationToken))
                .Where(c => dto.CollectionIds.Contains(c.Id)).ToList();

            var song = SongMapper.ToEntity(dto, user, authors, collections);

            await songRepository.CreateAsync(song, cancellationToken);

            var createdSong = await songRepository.GetByIdAsync(song.Id, cancellationToken) 
                ?? throw new KeyNotFoundException($"Произошла ошибка при создании песни");

            foreach (var variationId in dto.DefaultChordVariationIds) 
            {
                var chordVariation = await context.chordVariations.FindAsync(variationId);
                if (chordVariation == null)
                    throw new KeyNotFoundException($"Вариация аккорда с ID {variationId} не найдена");

                var defaultChordVariation = new DefaultChordVariation
                {
                    SongId = song.Id,
                    ChordVariationId = variationId,
                };
                await context.defaultChords.AddAsync(defaultChordVariation, cancellationToken);
            }
            await context.SaveChangesAsync(cancellationToken);

            return SongMapper.ToDto(createdSong);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException("Песня не найдена");
            await songRepository.DeleteAsync(song, cancellationToken);
        }

        public async Task DeleteFromFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(songId, cancellationToken)
                ?? throw new KeyNotFoundException($"Песня с Id {songId} не найдена");
            await songFavouriteRepository.DeleteFavouriteAsync(userId, songId, cancellationToken);
        }

        public async Task<List<SongDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var songs = await songRepository.GetAllAsync(cancellationToken);
            var songIds = songs.Select(s => s.Id).ToList();
            var favouritesCounts = await songFavouriteRepository.GetFavouritesCountForSongsAsync(songIds, cancellationToken);

            return songs.Select(song =>
                SongMapper.ToDto(song, favouritesCounts.TryGetValue(song.Id, out var count) ? count : 0)
            ).ToList();
        }

        public async Task<List<SongDto>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default)
        {
            var author = await authorRepository.GetByIdAsync(authorId, cancellationToken) 
                ?? throw new KeyNotFoundException($"Автор с ID {authorId} не найден");
            var songs = await songRepository.GetByAuthorIdAsync(authorId, cancellationToken);
            var songIds = songs.Select(s => s.Id).ToList();
            var favouritesCounts = await songFavouriteRepository.GetFavouritesCountForSongsAsync(songIds, cancellationToken);

            return songs.Select(song =>
                SongMapper.ToDto(song, favouritesCounts.TryGetValue(song.Id, out var count) ? count : 0)
            ).ToList();
        }

        public async Task<PagedResult<SongDto>> GetByCollectionPagedAsync(int collectionId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var collection = await collectionRepository.GetByIdAsync(collectionId, cancellationToken) 
                ?? throw new KeyNotFoundException($"Подборка с ID {collectionId} не найдена");
            var songs = await songRepository.GetByCollectionIdAsync(collectionId, cancellationToken);
            var songIds = songs.Select(s => s.Id).ToList();
            var favouritesCounts = await songFavouriteRepository.GetFavouritesCountForSongsAsync(songIds, cancellationToken);

            var sorted = songs.OrderByDescending(s => s.Views).ToList();
            var paged = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var items = paged.Select(song =>
                SongMapper.ToDto(song, favouritesCounts.TryGetValue(song.Id, out var count) ? count : 0)
            ).ToList();

            return new PagedResult<SongDto>
            {
                Items = items,
                TotalCount = sorted.Count
            };
        }

        public async Task<SongDto?> GetByIdAsync(int id, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var song = await songRepository.GetByIdAsync(id, cancellationToken) 
                ?? throw new KeyNotFoundException($"Песня с ID {id} не найдена");

            // Проверка доступа
            if (!song.IsPublic)
            {
                if (!userId.HasValue || song.UserId != userId.Value)
                {
                    // Не владелец и песня не публичная
                    throw new UnauthorizedAccessException("Нет доступа к этой песне");
                }
            }

            if (userId.HasValue)
            {
                bool hasViewed = await songViewRepository.HasUserViewedAsync(userId.Value, id, cancellationToken);
                if (!hasViewed)
                {
                    await songViewRepository.AddViewAsync(userId.Value, id, cancellationToken);
                    song.Views += 1;
                    await songRepository.UpdateAsync(song, cancellationToken);
                }
            }

            var favouritesCount = await songFavouriteRepository.GetFavouritesCountAsync(id, cancellationToken);

            return SongMapper.ToDto(song, favouritesCount); ;
        }

        public async Task<List<SongDto>> GetByUserIdAsync(Guid userId, Guid? currentUserId, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken) 
                ?? throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");
            List<Song> songs;
            if (currentUserId.HasValue && currentUserId.Value == userId)
            {
                // Запрашивает сам пользователь — показываем все его песни
                songs = await songRepository.GetAllByUserIdAsync(userId, cancellationToken);
            }
            else
            {
                // Запрашивает кто-то другой — только публичные
                songs = await songRepository.GetPublicByUserIdAsync(userId, cancellationToken);
            }
            var songIds = songs.Select(s => s.Id).ToList();
            var favouritesCounts = await songFavouriteRepository.GetFavouritesCountForSongsAsync(songIds, cancellationToken);

            return songs.Select(song =>
                SongMapper.ToDto(song, favouritesCounts.TryGetValue(song.Id, out var count) ? count : 0)
            ).ToList();
        }

        public async Task<List<SongDto>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var songs = await songFavouriteRepository.GetFavouritesAsync(userId, cancellationToken);
            var songIds = songs.Select(s => s.Id).ToList();
            var favouritesCounts = await songFavouriteRepository.GetFavouritesCountForSongsAsync(songIds, cancellationToken);

            return songs.Select(song =>
                SongMapper.ToDto(song, favouritesCounts.TryGetValue(song.Id, out var count) ? count : 0)
            ).ToList();
        }

        public async Task UpdateAsync(int id, SongCreateDto songCreateDto, CancellationToken cancellationToken = default)
        {
            SongValidator.Validate(songCreateDto);

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

        public async Task<PagedResult<SongDto>> GetPopularSongsPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var songs = await songRepository.GetAllAsync(cancellationToken);
            var songIds = songs.Select(s => s.Id).ToList();
            var favouritesCounts = await songFavouriteRepository.GetFavouritesCountForSongsAsync(songIds, cancellationToken);

            var sorted = songs.OrderByDescending(s => s.Views).ToList();
            var paged = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var items = paged.Select(song =>
                SongMapper.ToDto(song, favouritesCounts.TryGetValue(song.Id, out var count) ? count : 0)
            ).ToList();

            return new PagedResult<SongDto>
            {
                Items = items,
                TotalCount = sorted.Count
            };
        }
    }
}
