using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetUserByLoginAsync([FromRoute] string login, CancellationToken cancellationToken)
        {
            var user = await userService.GetUserByLoginAsync(login, cancellationToken);
            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken) 
        { 
            var users = await userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpPut("{login}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string login, [FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            await userService.UpdateAsync(login, userRegisterDto, cancellationToken);
            return Ok();
        }

        [HttpDelete("{login}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string login, CancellationToken cancellationToken)
        {
            await userService.DeleteAsync(login, cancellationToken);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            var (accessToken, refreshToken) = await userService.LoginUserAsync(userRegisterDto, cancellationToken);
            var accessCookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            };
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            if (userRegisterDto.RememberMe)
            {
                refreshCookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(30);
            }
            Response.Cookies.Append("access_token", accessToken, accessCookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];
            var (newAccessToken, newRefreshToken, isPersistent) = await userService.RefreshTokensAsync(refreshToken, cancellationToken);
            // Обновляем куки
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            };
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            };
            if (isPersistent)
            {
                refreshCookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(30);
                accessCookieOptions.Expires = DateTimeOffset.UtcNow.AddMinutes(15);
            }
            Response.Cookies.Append("access_token", newAccessToken, accessCookieOptions);
            Response.Cookies.Append("refresh_token", newRefreshToken, refreshCookieOptions);

            return Ok();
        }
    }
}
