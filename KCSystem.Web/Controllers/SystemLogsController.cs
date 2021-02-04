using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Core.Pages;
using KCSystem.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KCSystem.Web.Controllers
{
    public class SystemLogsController : Controller
    {
        private readonly IRepository<SystemLog> _systemLog;

        public SystemLogsController(IRepository<SystemLog> systemLog)
        {
            _systemLog = systemLog;
        }


        //[Resource("操作日志", Action = "查看")]
        public IActionResult Index()
        {
            return View();
        }
       // [Resource("操作日志", Action = "查看")]
        public IActionResult SystemLogs([FromBody] QueryParameters pages)
        {
            var query = _systemLog.Query(p => p.Disabled == false);

            if (!string.IsNullOrEmpty(pages.KeyWord))
            {
                query = query.Where(a => a.User.Phone.Contains(pages.KeyWord));
            }

            var data = query.Skip(pages.Offset).Take(pages.Limit)
                .Select(p => new
                {
                    p.Id,
                    p.User.Phone,
                    p.Scenarios,
                    p.Description,
                    p.AddTime,
                     
                }).ToList();
            var total = query.Count();
            return Json(new { rows = data, total });
        }
    }
}