using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KCSystem.Web.Models
{
    public class RolePowerModel
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PowerName { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 是否拥有权限
        /// </summary>
        public bool Checked { get; set; }
    }
}
