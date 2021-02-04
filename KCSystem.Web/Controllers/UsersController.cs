using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KCSystem.Common;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Core.Pages;
using KCSystem.Web.Extensions;
using KCSystem.Web.Models;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data; 

namespace KCSystem.Web.Controllers
{
    
    public class UsersController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager; 
        private readonly IRepository<Role> _roleRepository;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly ILogger<HomeController> _logger; 

        public UsersController(
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            ILogger<HomeController> logger, UserManager<User> userManager, IRepository<Role> roleRepository)
        {
            
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger; 
            _userManager = userManager;
            _roleRepository = roleRepository;
        }
        [Resource("用户管理",Action = "查看")]
        public IActionResult Index()
        {
            return View();
        } 
       

        [Resource("用户管理", Action = "编辑")]
        public ActionResult Add(int? id)
        {
            if (id == null || id.Value <= 0)
            {
                return View();
            }

            var model = _userRepository.Query(p=>p.Id == id).Include(p=>p.Leard).Include(p=>p.Role).FirstOrDefault();
                 
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Resource("用户管理", Action = "编辑")]
        public async Task<IActionResult> Add(User model)
        {
            if (ModelState.IsValid)
            {
                var hasModel = _userRepository.GetByKey( model.Id);
                if (model.Id > 0 && hasModel == null)
                {
                    ModelState.AddModelError("", "未找到记录!");
                }
                else
                { 
                  
                    if (model.Id > 0)
                    {
                        hasModel.RealName = model.RealName;
                        hasModel.Phone = model.Phone;
                        hasModel.RoleId = model.RoleId;
                        hasModel.LeardId = model.LeardId;
                        hasModel.Email = model.Email;
                        hasModel.DepartmentId = model.DepartmentId;
                        _userRepository.Update(hasModel);
                    }
                    else
                    {
                        if (_userRepository.QueryAll().Any(p => p.UserName == model.UserName && p.Disabled==false))
                        {
                            ModelState.AddModelError("", "该用户已经存在!");
                            return View();
                        }
                        else
                        {
                            model.AddTime = DateTime.Now;
                            await _userManager.CreateAsync(model, model.UserPassword);
                        }
                      
                    } 
                    var state = await _unitOfWork.SaveAsync();
                    if (state)
                    {

                        ViewBag.JS = "<script>dialogClose();</script>";
                    }
                    else
                    {

                        ModelState.AddModelError("", "保存失败，请与技术支持联系!");
                    }
                }
            }
            
            return View();
        }

        [Resource("用户管理", Action = "查看")]
        public IActionResult GetDataList([FromBody]QueryParameters pages)
        {
            var query = _userRepository.Query(p=>p.UserName !="Admin");
            if (pages.Disabled != null)
            {
                query = query.Where(p => p.Disabled == pages.Disabled
                );
            }
            if (!string.IsNullOrEmpty(pages.KeyWord))
            {
                query = query.Where(p => p.RealName.Contains(pages.KeyWord)
                                         || p.UserName.Contains(pages.KeyWord)
                                        );
            }

            
            var total = query.Count();
            var data=   query.Skip(pages.Offset).Take(pages.Limit)
                .Select(p=>new
                {
                    p.Id,p.UserName,p.RealName,p.Phone,p.AddTime,
                    RoleName =p.Role.Name, 
                    leard = p.Leard.RealName ,
                    Department=p.Department.EnumName
                }).ToList();
            return Json(new {rows = data, total});
        }
        [Resource("用户管理", Action = "删除")]
        public async Task<IActionResult> DeleteData(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json(new {Success = false, Msg = "非法请求"});
            }
            var idList = ids.ToList<int>();
            if (idList.Count == 0)
            {
                return Json(new { Success = false, Msg = "非法请求" });
            }
            var datas =_userRepository.QueryAll().Where(p => idList.Contains(p.Id)).ToList();
            var msg = "";
            foreach (var data in datas)
            {
                if (data.UserName != "Admin")
                {
                    data.Disabled = true;
                    _userRepository.Update(data);
                }
                else
                {
                    msg = "系统管理员不允许删除！";
                }
            }

            var state = await _unitOfWork.SaveAsync();
            return   Json(new { Success = state ,Msg = state?"删除成功！"+ msg : "删除失败！"+ msg });
        }


        [Resource("用户管理", Action = "编辑")]
        public async Task<IActionResult> GetUserDropdownList(string search)
        {

            var users = await _userRepository.Query(p => p.Disabled == false && p.RealName.Contains(search))
                .Take(5).Select(p=>new {p.Id,text=p.RealName}).ToListAsync();
            return Json(new { results=users} );
        }


        [Resource("用户管理", Action = "重置密码")]
        public IActionResult ChangePassword(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

        [Resource("用户管理", Action = "重置密码")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody]UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _userRepository.GetByKey(model.UserId);
            await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            var state = await _unitOfWork.SaveAsync();

            return Json(state);
        }

        [Resource("用户管理", Action = "编辑")]
        public async Task<IActionResult> GetRoleDropdownList(string search)
        {
            var users = await _roleRepository.Query(p => p.Disabled == false && p.Name.Contains(search))
                .Take(5).Select(p => new { Id=p.Id.ToString(), text = p.Name }).ToListAsync();
            users.Insert(0,new
            {
                Id="",
                text="请选择"

            });
            return Json(new { results = users });
        }
    }
}