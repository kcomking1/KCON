using System.Linq;
using System.Threading.Tasks;
using KCSystem.Common;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Core.Pages;
using KCSystem.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KCSystem.Web.Controllers
{


    public class ModulesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Module> _module;

        public ModulesController(
            IUnitOfWork unitOfWork,
            IRepository<Module> module)
        {


            _unitOfWork = unitOfWork;
            _module = module;

        }

        [Resource("模块管理", Action = "查看")]
        public IActionResult Index()
        {
            return View();
        }

        [Resource("模块管理", Action = "查看")]
        public IActionResult ModuleList([FromBody] QueryParameters pages)
        {
            var query = _module.Query(p => p.Disabled == false && p.Parent == null);

            if (!string.IsNullOrEmpty(pages.KeyWord))
            {
                query = query.Where(a => a.Parent.ModuleName.Contains(pages.KeyWord));
            }

            var data = query.OrderBy(p=>p.ModuleSortIndex).Include(p => p.Children).Skip(pages.Offset).Take(pages.Limit)
                .Select(p => new
                {
                    p.Id,
                    p.ModuleName,
                    p.ModuleUrl,
                    p.ModuleDesc,
                    p.ModuleSortIndex,
                    Children =p.Children.Where(c => c.Disabled == false).OrderBy(s=>s.ModuleSortIndex).ToList(),
                    p.Icon
                }).ToList();
            var total = query.Count();
            return Json(new {rows = data, total});
        }

        [Resource("模块管理", Action = "编辑")]
        public ActionResult Add(int? id)
        {
            var model = new Module();
            var query = _module.Query(p => p.Disabled == false);
            if (id != null && id > 0)
            {
                model = query.Include(p => p.Parent).FirstOrDefault(a => a.Id == id);

            }

            return View(model);
        }

        [HttpPost]
        [Resource("模块管理", Action = "编辑")]
        public async Task<IActionResult> Add([FromForm] Module model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    //修改
                    var module = _module.GetByKey(model.Id);
                    module.ModuleName = model.ModuleName;
                    module.ModuleDesc = model.ModuleDesc;
                    module.ModuleSortIndex = model.ModuleSortIndex;
                    module.ModuleUrl = model.ModuleUrl == null ? "" : model.ModuleUrl;
                    module.ModuleUse = model.ModuleUse;
                    module.ParentId = model.ParentId;
                    module.Icon = model.Icon;
                    _module.Update(module);
                }
                else
                {

                    await _module.InsertAsync(model);

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

            return View();
        }

        [Resource("模块管理", Action = "删除")]
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

            var query = _module.QueryAll();
            var datas = query.Where(p => idList.Contains(p.Id)).ToList();

            foreach (var data in datas)
            {

                data.Disabled = true;
                _module.Update(data);

            }

            var state = await _unitOfWork.SaveAsync();
            return Json(new {Success = state, Msg = state ? "删除成功！" : "删除失败！"});
        }

        [Resource("模块管理", Action = "编辑")]
        public async Task<IActionResult> GetParents(string search)
        {
            var users = await _module.Query(p => p.Disabled == false && p.ModuleName.Contains(search))
                .Take(5).Select(p => new {p.Id, text = p.ModuleName}).ToListAsync();
            return Json(new {results = users});
        }
    }


}
