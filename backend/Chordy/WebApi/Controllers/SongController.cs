﻿using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongController(ISongService songService, IAuthorizationService authorizationService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] SongCreateDto songCreateDto, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            songCreateDto.UserId = userId;

            var song = await songService.CreateAsync(songCreateDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = song.Id }, song);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaims = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userIdClaims, out var parsedId))
                    userId = parsedId;
            }

            var song = await songService.GetByIdAsync(id, userId, cancellationToken);
            return Ok(song);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var songs = await songService.GetAllAsync(cancellationToken);
            return Ok(songs);
        }

        [HttpGet("by-author/{authorId}")]
        public async Task<IActionResult> GetByAuthorId(int authorId, CancellationToken cancellationToken)
        {
            var songs = await songService.GetByAuthorIdAsync(authorId, cancellationToken);
            return Ok(songs);
        }

        [HttpGet("by-author/{authorId}/paged")]
        public async Task<IActionResult> GetByAuthorPaged(
            int authorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            CancellationToken cancellationToken = default)
        {
            var result = await songService.GetByAuthorIdPagedAsync(authorId, page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-collection/{collectionId}/paged")]
        public async Task<IActionResult> GetByCollectionPaged(
            int collectionId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            CancellationToken cancellationToken = default)
        {
            var result = await songService.GetByCollectionPagedAsync(collectionId, page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            Guid? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaims = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userIdClaims, out var parsedId))
                    currentUserId = parsedId;
            }

            var songs = await songService.GetByUserIdAsync(userId, currentUserId, cancellationToken);
            return Ok(songs);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] SongCreateDto songCreateDto, CancellationToken cancellationToken)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SongOwnerOrAdmin");
            if (!authorizationResult.Succeeded) {
                return Forbid();
            }

            await songService.UpdateAsync(id, songCreateDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SongOwnerOrAdmin");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            await songService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/favorite")]
        [Authorize]
        public async Task<IActionResult> AddToFavorites(int id, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await songService.AddToFavouriteAsync(userId, id, cancellationToken);
            return Ok();
        }

        [HttpDelete("{id}/favorite")]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavorites(int id, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await songService.DeleteFromFavouriteAsync(userId, id, cancellationToken);
            return Ok();
        }

        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetFavorites(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var songs = await songService.GetFavouritesAsync(userId, cancellationToken);
            return Ok(songs);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularSongs([FromQuery] int page = 1, [FromQuery] int pageSize = 15, CancellationToken cancellationToken = default) {
            var result = await songService.GetPopularSongsPagedAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSongs([FromQuery] string query, CancellationToken cancellationToken)
        {
            var songs = await songService.SearchSongsByNameAsync(query, cancellationToken);
            return Ok(songs);
        }
    }
}
