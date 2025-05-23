﻿using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface ISongService
    {
        Task<SongDto> CreateAsync(SongCreateDto dto, CancellationToken cancellationToken = default);
        Task<SongDto?> GetByIdAsync(int id, Guid? userId = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, SongCreateDto songCreateDto, CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetByUserIdAsync(Guid userId, Guid? currentUserId, CancellationToken cancellationToken = default);
        Task<PagedResult<SongDto>> GetByCollectionPagedAsync(int collectionId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default);
        Task AddToFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
        Task DeleteFromFavouriteAsync(Guid userId, int songId, CancellationToken cancellationToken = default);
        Task<List<SongDto>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<PagedResult<SongDto>> GetPopularSongsPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<SongDto>> GetByAuthorIdPagedAsync(int authorId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<List<SongDto>> SearchSongsByNameAsync(string query, CancellationToken cancellationToken = default);
    }
}
