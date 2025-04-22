using Chordy.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Chordy.BusinessLogic.Authorization
{
    public class SongOwnerOrAdminRequirements : IAuthorizationRequirement { }

    public class SongOwnerOrAdminHandler(ISongService songService) : AuthorizationHandler<SongOwnerOrAdminRequirements, int>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SongOwnerOrAdminRequirements requirements, int songId)
        {
            var userIdClaim = context.User.FindFirst("userId")?.Value;
            var isAdmin = context.User.IsInRole("admin");

            if (isAdmin)
            {
                context.Succeed(requirements);
                return;
            }

            if (Guid.TryParse(userIdClaim, out var userId)) 
            {
                var song = await songService.GetByIdAsync(songId);
                if (song != null && song.UserId == userId)
                {
                    context.Succeed(requirements);
                }
            }
        }
    }
}
