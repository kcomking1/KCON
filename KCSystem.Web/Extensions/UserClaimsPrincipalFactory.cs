using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using Microsoft.EntityFrameworkCore;

namespace KCSystem.Web.Extensions
{
    public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
    {
        private readonly IRepository<RolePower> _powerRepository;
        private readonly IRepository<User> _userRepository;
        public UserClaimsPrincipalFactory(IRepository<RolePower> powerRepository, IRepository<User> userRepository)
        {
            _powerRepository = powerRepository;
            _userRepository = userRepository;
        }

        public Task<ClaimsPrincipal> CreateAsync(User user)
        {
            user = _userRepository.Query(p => p.Id == user.Id).Include(p=>p.Role).FirstOrDefault();
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Name, user.RealName, ClaimValueTypes.String),
                new Claim("RoleId",user.RoleId==null?"":user.RoleId.ToString()),
                new Claim(ClaimTypes.Role,user.RoleId==null?"":user.Role.Name),
                new Claim("SigninTime", System.DateTime.UtcNow.Ticks.ToString(), ClaimValueTypes.Integer64)
            };
           
            
            if (user.UserName.ToLower() != "admin")
            {
                var list = _powerRepository.Query(a => a.Role.Disabled == false && a.RoleId == user.RoleId);
                foreach (var policy in list)
                {
                    claims.Add(new Claim(policy.PowerName, policy.Action ?? ""));
                }
            }
            else
            {
                foreach (var item in ResourceData.Resources)
                {
                    if (item.Value.Any())
                    {
                        foreach (var i in item.Value)
                        {
                            claims.Add(new Claim(item.Key, i));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(item.Key, ""));
                    }
                }
            }
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            
            return Task.FromResult(new ClaimsPrincipal(identity));
        }
    }
}