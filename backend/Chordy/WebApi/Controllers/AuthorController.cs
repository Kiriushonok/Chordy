using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController(IAuthorService authorService, ILogger<AuthorController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAuthorAsync([FromBody] AuthorDto authorDto, CancellationToken cancellationToken)
        {
            var author = await authorService.CreateAsync(authorDto.Name, cancellationToken);
            return Created($"/Author/{author.id}", author.name);
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
                var authorName = await authorService.GetAuthorNameByIdAsync(id, cancellationToken);
                return Ok(authorName);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetAuthorByNameAsync([FromRoute] string name, CancellationToken cancellationToken)
        {
            var authorName = await authorService.GetByNameAsync(name, cancellationToken);
            return Ok(authorName);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthorAsync([FromRoute] int id, [FromBody] string newText, CancellationToken cancellationToken)
        {
                await authorService.UpdateAsync(id, newText, cancellationToken);
                return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthorAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
                await authorService.DeleteAsync(id, cancellationToken);
                return NoContent();
        }
    }
}
