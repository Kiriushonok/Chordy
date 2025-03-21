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
        public async Task<IActionResult> CreateAsync([FromBody] AuthorDto authorDto)
        {
            var author = await authorService.CreateAsync(authorDto.Name);
            return CreatedAtAction(nameof(CreateAsync), new { id = author.id }, author);
        }
    }
}
