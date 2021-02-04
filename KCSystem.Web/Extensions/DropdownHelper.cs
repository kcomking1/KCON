using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Infrastructrue.Database;

namespace KCSystem.Web.Extensions
{
    public  class DropdownHelper
    {
        private readonly IRepository<BackEnum> _db;
        private readonly IRepository<User> _userDb;
       
        private readonly IRepository<Role> _role; 
        public DropdownHelper(IRepository<BackEnum> backEnum, IRepository<User> userDb, IRepository<Role> role 
             )
        {
            _db = backEnum;
            _userDb = userDb; 
            _role = role; 
        }

        public  List<SelectListItem> GetEnumByParentName(string name)
        {
            var datas=_db.Query(p => p.Disabled == false && p.Parent.EnumName == name).OrderBy(p => p.Sort).ToList();

            return datas.Select(p => new SelectListItem(p.EnumName,p.Id.ToString())).ToList();
        }

        public List<SelectListItem> GetEnumByParentName(string name,string defaultName)
        {
            var datas = _db.Query(p => p.Disabled == false && p.Parent.EnumName == name).OrderBy(p => p.Sort).ToList();

            var items = datas.Select(p => new SelectListItem(p.EnumName, p.Id.ToString())).ToList();
            items.Insert(0, new SelectListItem(defaultName, ""));
            return items;
        }

        

        /// <summary>
        /// 获取字典数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="backEnums"></param>
        public  static BackEnum GetBackEnum(string enumParent,string item, Dictionary<string, List<BackEnum>> backEnums, IRepository<BackEnum> _db)
        {
            List<BackEnum> enums = null;
            if (!backEnums.Keys.Contains(enumParent))
            { 
              enums = _db.Query(p =>p.Disabled==false && p.Parent.EnumName == enumParent).OrderBy(p => p.Sort).ToList();
              backEnums.Add(enumParent, enums);
            }
            else
            {
                enums = backEnums[enumParent];
            }

            var backEnum = enums.FirstOrDefault(p => p.EnumName == item);
            return backEnum;
        }

        public List<SelectListItem> GetManagers(string roleName)
        {
            var datas = _userDb.Query(p => p.Disabled == false && p.Role.Name == roleName).ToList();

            return datas.Select(p => new SelectListItem(p.RealName, p.Id.ToString())).ToList();
        }

        public List<SelectListItem> GetRoles()
        {
            var datas = _role.Query(p => p.Disabled == false ).ToList();

            return datas.Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList();
        }

        
    }
}
