using Chordy.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/main-info")]
    public class MainInfoController(
        ISongService songService,
        IAuthorService authorService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMainPageData(CancellationToken cancellationToken)
        {
            var popularSongs = (await songService.GetAllAsync(cancellationToken))
                .OrderByDescending(s => s.Views)
                .Take(10)
                .ToList();

            var newSongs = (await songService.GetAllAsync(cancellationToken))
                .OrderByDescending(s => s.Date)
                .Take(10)
                .ToList();

            var popularAuthors = await authorService.GetTopAuthorsByViewsAsync(10, cancellationToken);

            return Ok(new
            {
                popularSongs,
                popularAuthors,
                newSongs
            });
        }
    }
}
