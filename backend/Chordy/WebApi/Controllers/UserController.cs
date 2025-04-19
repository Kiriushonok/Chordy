using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService userService) : Controller
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            var user = await userService.CreateAsync(userRegisterDto, cancellationToken);
            return CreatedAtAction("GetUserByLogin", new {login = userRegisterDto.Login}, user);
        }

        [HttpGet("{login}")]
        public async Task<IActionResult> GetUserByLoginAsync([FromRoute] string login, CancellationToken cancellationToken)
        {
            var user = await userService.GetUserByLoginAsync(login);
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken) 
        { 
            var users = await userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpPut("{login}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string login, [FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            await userService.UpdateAsync(login, userRegisterDto, cancellationToken);
            return Ok();
        }

        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string login, CancellationToken cancellationToken)
        {
            await userService.DeleteAsync(login, cancellationToken);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            var token = await userService.LoginUserAsync(userRegisterDto, cancellationToken);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            };
            Response.Cookies.Append("tasty-cookie", token, cookieOptions);
            return Ok(token);
        }
    }
}
