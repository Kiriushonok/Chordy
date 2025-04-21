using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongController(ISongService songService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] SongCreateDto songCreateDto, CancellationToken cancellationToken) 
        {
            var song = await songService.CreateAsync(songCreateDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = song.Id }, song);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var song = await songService.GetByIdAsync(id, cancellationToken);
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

        [HttpGet("by-collection/{collectionId}")]
        public async Task<IActionResult> GetByCollectionId(int collectionId, CancellationToken cancellationToken)
        {
            var songs = await songService.GetByCollectionIdAsync(collectionId, cancellationToken);
            return Ok(songs);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var songs = await songService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(songs);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] SongCreateDto songCreateDto, CancellationToken cancellationToken)
        {
            await songService.UpdateAsync(id, songCreateDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await songService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
