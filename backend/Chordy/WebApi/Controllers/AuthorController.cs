using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController(IAuthorService authorService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAuthorAsync([FromForm] AuthorCreateDto authorCreateDto, CancellationToken cancellationToken)
        {
            var author = await authorService.CreateAsync(authorCreateDto, cancellationToken);
            return CreatedAtAction("GetById", new { id = author.Id }, author);
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var authorName = await authorService.GetAuthorByIdAsync(id, cancellationToken);
            return Ok(authorName);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetAuthorByNameAsync([FromRoute] string name, CancellationToken cancellationToken)
        {
            var authorName = await authorService.GetByNameAsync(name, cancellationToken);
            return Ok(authorName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var authors = await authorService.GetAllAsync(cancellationToken);
            return Ok(authors);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateAuthorAsync([FromRoute] int id, [FromForm] AuthorCreateDto authorCreateDto, CancellationToken cancellationToken)
        {
            await authorService.UpdateAsync(id, authorCreateDto, cancellationToken);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAuthorAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await authorService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedAuthors([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var paged = await authorService.GetPagedAuthorsAsync(page, pageSize, cancellationToken);
            return Ok(paged);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAuthors([FromQuery] string query, CancellationToken cancellationToken)
        {
            var authors = await authorService.SearchAuthorsByNameAsync(query, cancellationToken);
            return Ok(authors);
        }
    }
}
