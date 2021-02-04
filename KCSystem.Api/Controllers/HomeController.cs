using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KCSystem.Api.AuthHelper; 
using KCSystem.Api.Models;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace KCSystem.Api.Controllers
{
    [ApiController]
    [Route("api/home")] 
    [EnableCors("LimitRequests")]
     
    public class HomeController : Controller
    {
        
        private readonly IRepository<User> _userRepository; 
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly ITypeHelperService _typeHelperService; 
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IRepository<RolePower> _powerRepository;
        private readonly IRepository<Module> _module;
        private readonly IMemoryCache _cache;
        public HomeController( 
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            ILogger<HomeController> logger, ITypeHelperService typeHelperService,    IPasswordHasher<User> passwordHasher, IConfiguration configuration, IRepository<RolePower> powerRepository, IRepository<Module> module, IMemoryCache cache)
        {
          
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _typeHelperService = typeHelperService;
            
           
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _powerRepository = powerRepository;
            _module = module;
            _cache = cache;
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("/Test")]
        public IActionResult Test()
        {

            return Ok(1);
        }

        /// <summary>
        /// Notice
        /// </summary>
        /// <returns></returns>
        [Resource("系统管理", Action="通知")]
        [HttpGet] 
        public IActionResult Notice()
        {
            
            return Ok();
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        
        [HttpGet("/OperatorMenu")]
        [Resource] 
        public ActionResult<List<Menu>> GetOperatorMenu()
        {
            var menus = new List<Menu>();
            var claims = HttpContext.User.Claims.Select(p => p.Type).Distinct().ToList();
            var query = _module.Query(a => a.Disabled == false);

            if (User.Identity.Name.ToUpper() != "ADMIN")
            {
                query = query.Where(p => (p.ModuleUrl == "") || claims.Contains(p.ModuleName));
            }

            var modules = query.ToList();
            var parents = modules.Where(p => p.ParentId == null).OrderBy(p => p.ModuleSortIndex).ToList();
            foreach (var item in parents)
            {
                Menu menu = new Menu();
                var ml = new List<Menu>();
                var children = modules.Where(a => a.ParentId == item.Id).OrderBy(p => p.ModuleSortIndex).ToList();
                if (children.Any()) //有子数据
                {
                    foreach (var lc in children)
                    {
                        ml.Add(new Menu
                        {
                            Icon = lc.Icon,
                            Url = (lc.ModuleUrl),
                            Text = lc.ModuleName,
                        });
                    }
                }
                else if (item.ModuleUrl == "")
                {
                    continue;
                }


                menu.Icon = item.Icon;
                menu.Url = (item.ModuleUrl);
                menu.Text = item.ModuleName;
                menu.Children = ml;
                menus.Add(menu);
            }


            return menus;
        }


        [Resource]
        [HttpPost]
        [Route("/ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var adminUser = _userRepository.GetByKey(model.UserId.Value);
            if (adminUser == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    Message = "修改失败"
                });
            }
            var pwdVerification = _passwordHasher.VerifyHashedPassword(
                adminUser,
                adminUser.UserPassword,
                model.CurrentPassword);

            if (pwdVerification == PasswordVerificationResult.Success ||
                pwdVerification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                adminUser.UserPassword = _passwordHasher.HashPassword(adminUser, model.NewPassword);
                _userRepository.Update(adminUser);
                var state = await _unitOfWork.SaveAsync();
                return Json(state);
            }  
            return Json(false);
        }

        

        [HttpPost] 
        [Route("/api/signin")]
        public async Task<IActionResult> DoSignin([FromBody]LoginViewModel viewModel, [FromQuery]string returnUrl)
        {
            var adminUser = await _userRepository.QueryAll().Include(p => p.Role).FirstOrDefaultAsync(u => u.UserName.ToLower() == viewModel.UserName.ToLower());
            if (adminUser == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    Message = "认证失败"
                });
            } 
            var pwdVerification = _passwordHasher.VerifyHashedPassword(
                adminUser,
                adminUser.UserPassword,
                viewModel.Password);
             
            if (pwdVerification == PasswordVerificationResult.Success ||
                pwdVerification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                 
                var claims = new List<Claim>(); 
                if (adminUser.UserName.ToLower() != "admin")
                {
                    var list = await _powerRepository.Query(a => a.Role.Disabled == false && a.RoleId == adminUser.RoleId).ToListAsync();
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
                _cache.Set(adminUser.Id, claims,DateTimeOffset.Now.AddDays(1));
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Audience:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,adminUser.Id.ToString()),
                        new Claim(ClaimTypes.Name, adminUser.RealName)
                    }),
                    Issuer = _configuration["Audience:Issuer"],
                    Audience = _configuration["Audience:Audience"],
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };
                tokenDescriptor.Subject.AddClaims(claims);
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return Json(new
                {
                    adminUser.Id,
                    Name = adminUser.RealName,
                    success = true,
                    token = tokenString
                });
            }
            else
            {
                return new JsonResult(new
                {
                    
                    success = false,
                    Message = "认证失败"
                });
            }
        }
 
    }
}
