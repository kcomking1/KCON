using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KCSystem.Common;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Web.Extensions;
using KCSystem.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; 
using NPOI.SS.Formula.Functions;
using Microsoft.EntityFrameworkCore;
using KCSystem.Core.Enum;
using Microsoft.Extensions.Configuration;

namespace KCSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Module> _module;  
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IRepository<User> userRepository,  
            IUnitOfWork unitOfWork,
            ILogger<HomeController> logger, IRepository<Module> module, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository; 
            _unitOfWork = unitOfWork;
            _logger = logger;
            _module = module;
            _hostingEnvironment = hostingEnvironment; 
            _configuration = configuration;
        }



        // [Authorize]
        // GET: ConfigSets
        public IActionResult Index()
        {
            if (!HttpContext.IsAuthenticated())
            {

                return RedirectTo("/signin");
            }
            else
            {

                ViewBag.Menus = GetOperatorMenu().ToJson();
                ViewBag.OpenWater = _configuration.GetValue<int>("OpenWater");
                return View();
            } 

        }

       

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        private List<Menu> GetOperatorMenu()
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
                var children = modules.Where(a => a.ParentId == item.Id).OrderBy(p => p.ModuleSortIndex).ToList();
                
                Menu menu = new Menu();
                var ml = new List<Menu>();
                
                if (children.Any()) //有子数据
                {
                    foreach (var lc in children)
                    {
                        if (!children.Any(p => HttpContext.User.Claims.Any(s=>s.Type == lc.ModuleName && s.Value =="查看")))
                        {
                            continue;
                        }
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

        [Route("/UploadTest")]
        public void UploadTest() {
            var file = Request.Form.Files[0];//["filename"];
            if (file != null && file.Name != "")
            {
                var path = _hostingEnvironment.WebRootPath;
                var uploadDir = "";
                var fullDir = Path.Combine(path, uploadDir);
                if (!Directory.Exists(fullDir))
                {
                    Directory.CreateDirectory(fullDir);
                }
                var filename = "webOfficeDoc.doc";
                var fullPath = Path.Combine(path, uploadDir, filename);
                using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    file.CopyTo(fs);
                    fs.Close();
                }
                Response.WriteAsync("ok");
            }
        }
        
        [Authorize]
        public IActionResult ChangePassword() 
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = User.ToUser(_userRepository);
            await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            var state = await _unitOfWork.SaveAsync();

            return Json(state);
        }

        [Route("/signin")]
        public IActionResult Signin([FromQuery] string returnUrl)
        {
            if (HttpContext.IsAuthenticated())
            {
                return RedirectTo(returnUrl);
            }


            return View();
        }

        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {


            return View();
        }


        [HttpPost]
        [Route("/signin")]
        public async Task<IActionResult> DoSignin([FromForm] LoginViewModel viewModel, [FromQuery] string returnUrl)
        {
            if (HttpContext.IsAuthenticated())
            {
                return RedirectTo(returnUrl);
            }

            var result = Microsoft.AspNetCore.Identity.SignInResult.Failed;
            if (ModelState.IsValid)
            {

                result = await _signInManager.PasswordSignInAsync(
                    viewModel.UserName,
                    viewModel.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);
                var logLevel = result.Succeeded ? LogLevel.Information : LogLevel.Warning;
                var resultDesc = result.Succeeded ? "成功" : "失败";
                _logger.Log(logLevel, $"用户登录{resultDesc}：用户名 {viewModel.UserName}：{result}");


            }
            else
            {
                _logger.LogWarning($"用户登录失败：用户名 {viewModel.UserName}：数据格式不正确。");
            }

            if (!result.Succeeded)
            {

                ModelState.Clear(); // 将真正的验证结果隐藏掉（如果有的话）
                ModelState.AddModelError("UserName", "用户名或密码错误"); 
                return View("Signin");
            }

            //if (User.ExtractUserRoleName().Contains("清收"))
            { 
                var b = new Regex(
                    @"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var v = new Regex(
                    @"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);


                var userAgent = Request.Headers["User-Agent"].ToString();
                if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
                {
                    return RedirectTo("/Mobile/index.html");
                }

            }

            return RedirectTo(returnUrl);
        }

        [HttpPost]
        [Route("/signout")]
        [Authorize]
        public async Task<IActionResult> DoSignOut()
        {
            await _signInManager.SignOutAsync();
            return Json(new {success = true}); 
        }


        private IActionResult RedirectTo(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }

            return Redirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                {RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpPost]
        [Authorize]
        [Route("/TinyMCE/Upload")]
        public string Upload()
        {
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                return "No File";
            }

            var file = files[0];
            var path = _hostingEnvironment.WebRootPath;
            var uploadDir = "UploadFiles/" + DateTime.Now.ToString("yyyyMMdd");
            var fullDir = Path.Combine(path, uploadDir);
            if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);
            var filename = file.FileName;
            var fullPath = Path.Combine(path, uploadDir, filename);
            using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fs);
                fs.Close();
            }

            var url = $"/{uploadDir}/{filename}";
            var fi = new {location = url};
            return fi.ToJson();
        }

         

        [Route("/homeIndex")]
        public IActionResult HomeIndex()
        {
            return View();
        }

        

      
    }
}
