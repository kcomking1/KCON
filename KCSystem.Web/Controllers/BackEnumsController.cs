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


    public class BackEnumsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<BackEnum> _backEnum;

        public BackEnumsController(
            IUnitOfWork unitOfWork,
            IRepository<BackEnum> backEnum)
        {


            _unitOfWork = unitOfWork;
            _backEnum = backEnum;

        }

        [Resource("字典管理", Action = "查看")]
        public IActionResult Index()
        {
            return View();
        }

        [Resource("字典管理", Action = "查看")]
        public IActionResult BackEnumsList([FromBody] QueryParameters pages)
        {
            var query = _backEnum.Query(p => p.Disabled == false && p.Parent == null);

            if (!string.IsNullOrEmpty(pages.KeyWord))
            {
                query = query.Where(a => a.EnumName.Contains(pages.KeyWord)||a.Parent.EnumName.Contains(pages.KeyWord));
            }

            var data = query.Include(p => p.Children).Skip(pages.Offset).Take(pages.Limit)
                .Select(p => new
                {
                    p.Id,
                    p.EnumName,
                    p.Sort,p.Remark,
                    ParentName=p.Parent.EnumName,
                    Children =p.Children.Where(c => c.Disabled == false).OrderBy(c=>c.Sort).ToList()
                }).ToList();
            var total = query.Count();
            return Json(new {rows = data, total});
        }

        [Resource("字典管理", Action = "编辑")]
        public ActionResult Add(int? id)
        { 
          
            if (id == null || !(id > 0)) return View();
            {
                var query = _backEnum.Query(p => p.Disabled == false);
                var model = query.Include(p => p.Parent).FirstOrDefault(a => a.Id == id);
                return View(model);
            }

        }

        [HttpPost]
        [Resource("字典管理", Action = "编辑")]
        public async Task<IActionResult> Add([FromForm] BackEnum model)
        {
            if (!ModelState.IsValid) return View();
            if (model.Id > 0)
            {
                //修改
                var backEnum = _backEnum.GetByKey(model.Id);
                backEnum.ParentId = model.ParentId;
                backEnum.EnumName = model.EnumName;
                backEnum.Remark = model.Remark;
                backEnum.Sort = model.Sort;
                _backEnum.Update(backEnum);
            }
            else
            { 
                await _backEnum.InsertAsync(model); 
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

            return View();
        }

        [Resource("字典管理", Action = "删除")]
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

            var query = _backEnum.QueryAll();
            var datas = query.Where(p => idList.Contains(p.Id)).ToList();

            foreach (var data in datas)
            {

                data.Disabled = true;
                _backEnum.Update(data);

            }

            var state = await _unitOfWork.SaveAsync();
            return Json(new {Success = state, Msg = state ? "删除成功！" : "删除失败！"});
        }

        [Resource("字典管理", Action = "编辑")]
        public async Task<IActionResult> GetParents(string search)
        {
            var users = await _backEnum.Query(p => p.Disabled == false && p.ParentId == null && p.EnumName.Contains(search))
                .Take(5).Select(p => new {p.Id, text = p.EnumName }).ToListAsync();
            return Json(new {results = users});
        }
    }


}
