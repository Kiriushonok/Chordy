using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Chordy.BusinessLogic.Authorization
{
    public class ChordVariationOwnerOrAdminRequirements : IAuthorizationRequirement { }

    public class ChordVariationOwnerOrAdminHandler(IChordVariationRepository repo) : AuthorizationHandler<ChordVariationOwnerOrAdminRequirements, int>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ChordVariationOwnerOrAdminRequirements requirements, int variationId)
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
                var variation = await repo.GetByIdAsync(variationId);
                if (variation != null && variation.UserId == userId)
                {
                    context.Succeed(requirements);
                }
            }
        }
    }
}