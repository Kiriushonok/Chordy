using Chordy.BusinessLogic.Interfaces;
using Chordy.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController(IAuthorService authorService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAuthorAsync([FromBody] AuthorDto authorDto)
        {
            var author = await authorService.CreateAsync(authorDto.Name);
            return Created($"/Author/{author.id}", author.name);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorAsync([FromRoute] int id)
        {
            var authorName = await authorService.GetByIdAsync(id);
            return Ok(authorName);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthorAsync([FromRoute] int id, [FromBody] string newText)
        {
            await authorService.UpdateAsync(id, newText);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthorAsync([FromRoute] int id)
        {
            await authorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
