using System.Linq;
using System.Security.Claims;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KCSystem.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsAuthenticated(this HttpContext httpContext)
        {
            return IsAuthenticated(httpContext?.User); 
        }
        
        public static bool IsAuthenticated(this ClaimsPrincipal claimsPrincipal)
        {
            var isAuthedExpr = claimsPrincipal?.Identity?.IsAuthenticated;
            return isAuthedExpr.HasValue && isAuthedExpr.Value; 
        }
        
        public static User DiscussionUser(this HttpContext httpContext)
        {
            if (!IsAuthenticated(httpContext))
            {
                return null;
            }

            return ToUser(httpContext.User,  httpContext.RequestServices.GetRequiredService<IRepository<User>>());
        }
        
        public static User ToUser(this ClaimsPrincipal claimsPrincipal, IRepository<User> userRepo)
        {
            var userId = ExtractUserId(claimsPrincipal);
            return userId == null ? null : userRepo.GetByKey(userId.Value);
        }

        public static int? ExtractUserId(this ClaimsPrincipal claimsPrincipal)
        {
            bool IsIdClaim(Claim claim)
            {
                return claim.Type == ClaimTypes.NameIdentifier;
            }

            var identity = claimsPrincipal.Identities.FirstOrDefault(id => id.HasClaim(IsIdClaim));
            var userIdClaim = identity?.Claims.FirstOrDefault(IsIdClaim)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
