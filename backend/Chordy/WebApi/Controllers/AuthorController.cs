using Chordy.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController(IAuthorService authorService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync(string name)
        {
            await authorService.CreateAsync(name);
            return NoContent();
        }
    }
}
