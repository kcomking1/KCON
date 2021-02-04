using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Core.Pages;
using KCSystem.Web.Extensions;
using KCSystem.Common;
using KCSystem.Web.Models;

namespace KCSystem.Web.Controllers
{

    public class RolesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Role> _role;
        private readonly IRepository<RolePower> _zxPower;
        private readonly IRepository<Module> _module;

        public RolesController(
            IUnitOfWork unitOfWork, IRepository<Module> zxModule, IRepository<RolePower> zxPower,
            IRepository<Role> role, IRepository<Module> module)
        {
            _unitOfWork = unitOfWork;
            _zxPower = zxPower;
            _role = role;
            _module = module;
        }

        [Resource("角色管理", Action = "查看")]
        public IActionResult Index()
        {

            return View();
        }

        [Resource("角色管理", Action = "查看")]
        public IActionResult RoleModuleList([FromBody] QueryParameters pages)
        {
            var query = _role.Query(p => p.Disabled == false);
            if (!string.IsNullOrEmpty(pages.KeyWord))
            {
                query = query.Where(p => p.Name.Contains(pages.KeyWord));
            }
            var total = query.Count();
            var data = query.Skip(pages.Offset).Take(pages.Limit)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Remark,
                    ParentName = p.Parent.Name,
                    p.AddTime
                }).ToList();
            return Json(new {rows = data, total});
        }

        [Resource("角色管理", Action = "编辑权限")]
        public IActionResult AddRole(int id)
        {
            //获取菜单
            var menus = _module.QueryAll().Include(p => p.Parent).ToList();
            var hasRoles = _zxPower.Query(a => a.Disabled == false && a.RoleId == id).ToList();
            var roles = new List<dynamic>();
            foreach (var key in ResourceData.Resources.Keys)
            {
                var actions = ResourceData.Resources[key].Select(p => new RolePowerModel()
                {
                    PowerName = key,
                    Checked = hasRoles.Any(s => s.Action == p && s.PowerName == key),
                    Action = p,
                    RoleId = id

                }).ToList();
                var group = menus.FirstOrDefault(p => p.ModuleName == key);
                roles.Add(
                    new
                    {
                        Group = group ==null ?"其它":group.Parent == null ?group.ModuleName:group.Parent.ModuleName,
                        Order = group == null ? 999: group.Parent == null ? group.ModuleSortIndex : group.Parent.ModuleSortIndex,
                        key,
                        Checked = false,
                        actions
                    });
            }

           
            ViewBag.Resouces = roles.OrderBy(p=>p.Order).ToJson();
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        [Resource("角色管理", Action = "编辑权限")]
        public async Task<IActionResult> AddRole([FromBody] List<RolePowerModel> models)
        {
            var id = models.First().RoleId;
            var list = _zxPower.Query(a => a.Disabled == false && a.RoleId == id).ToList();
            foreach (var data in list)
            {
                _zxPower.Remove(data);
            }
            
            foreach (var data in models)
            {
                var rolePower = new RolePower
                {
                    PowerName = data.PowerName,
                    Action = data.Action,
                    RoleId = data.RoleId
                };
                _zxPower.Insert(rolePower);
            }

            var state = await _unitOfWork.SaveAsync();
            return Json(new {result = state, msg = "系统异常，请稍候重试！"});
        }

        [Resource("角色管理", Action = "编辑")]
        public IActionResult Add(int? id)
        {
            if (id == null || id.Value <= 0)
            {
                return View();
            }

            var model = _role.Query(p => p.Id == id).Include(p => p.Parent).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        [Resource("角色管理", Action = "编辑")]
        public async Task<IActionResult> Add([FromForm] Role model)
        {

            if (ModelState.IsValid)
            {
                var hasModel = _role.GetByKey(model.Id);
                if (model.Id > 0 && hasModel == null)
                {
                    ModelState.AddModelError("", "未找到记录!");
                }
                else
                {

                    if (model.Id > 0)
                    {
                        hasModel.ParentId = model.ParentId;
                        hasModel.Name = model.Name;

                        hasModel.Remark = model.Remark;

                        _role.Update(hasModel);
                    }
                    else
                    {
                        if (_role.QueryAll().Any(p =>
                            p.Disabled == false && p.Name == model.Name && p.ParentId == model.ParentId))
                        {
                            ModelState.AddModelError("Name", "该用户已经存在!");
                            return View(model);
                        }
                        else
                        {
                            model.AddTime = DateTime.Now;
                            _role.Insert(model);
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

        [Resource("角色管理", Action = "删除")]
        public async Task<IActionResult> DeleteData(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json(new {Success = false, Msg = "非法请求"});
            }

            var idList = ids.ToList<int>();
            if (idList.Count == 0)
            {
                return Json(new {Success = false, Msg = "非法请求"});
            }

            var datas = _role.QueryAll().Where(p => idList.Contains(p.Id)).ToList();
            var msg = "";
            foreach (var data in datas)
            {

                data.Disabled = true;
                _role.Update(data);
            }

            var state = await _unitOfWork.SaveAsync();
            return Json(new {Success = state, Msg = state ? "删除成功！" + msg : "删除失败！" + msg});
        }

        [Resource("角色管理", Action = "编辑")]
        public async Task<IActionResult> GetParentList(string search)
        {

            var roles = await _role.Query(p => p.Disabled == false && p.Name.Contains(search))
                .Take(5).Select(p => new {p.Id, text = p.Name}).ToListAsync();
            return Json(new {results = roles});
        }
    }
}
