using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;

namespace KCSystem.Web.Extensions
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
        
        public static User ManageUser(this HttpContext httpContext)
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
            return userId == null ? null : userRepo.Query(p=>p.Id == userId.Value).FirstOrDefault();
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

        public static int? ExtractUserRoleId(this ClaimsPrincipal claimsPrincipal)
        {
            bool IsRoleClaim(Claim claim)
            {
                return claim.Type == "RoleId";
            }

            var identity = claimsPrincipal.Identities.FirstOrDefault(id => id.HasClaim(IsRoleClaim));
            var userRoleClaim = identity?.Claims.FirstOrDefault(IsRoleClaim)?.Value;
            if (userRoleClaim == null || !int.TryParse(userRoleClaim, out var roleId))
            {
                return null;
            }

            return roleId;
        }

        public static string ExtractUserRoleName(this ClaimsPrincipal claimsPrincipal)
        {
            bool IsRoleClaim(Claim claim)
            {
                return claim.Type == ClaimTypes.Role;
            }

            var identity = claimsPrincipal.Identities.FirstOrDefault(id => id.HasClaim(IsRoleClaim));
            var userRoleClaim = identity?.Claims.FirstOrDefault(IsRoleClaim)?.Value;
            

            return string.IsNullOrEmpty(userRoleClaim)?"管理员":userRoleClaim;
        }
    }
}
