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
            try {

            var author = await authorService.CreateAsync(authorDto.Name, cancellationToken);
            return Created($"/Author/{author.id}", author.name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при создании автора: {ErrorMessage}", ex.Message);
                return Problem("Произошла ошибка при создании автора", statusCode: 500);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var authorName = await authorService.GetByIdAsync(id, cancellationToken);
                return Ok(authorName);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthorAsync([FromRoute] int id, [FromBody] string newText, CancellationToken cancellationToken)
        {
            try
            {
                await authorService.UpdateAsync(id, newText, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthorAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                await authorService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
