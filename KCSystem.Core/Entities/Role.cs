using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCSystem.Core.Entities
{
    /// <summary>
    /// 权限
    /// </summary>
   public class Role:Entity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(20, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "角色名")]
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        public int? ParentId { get; set; }

        public Role Parent { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Role> Children { get; set; }
        public IEnumerable<RolePower> Powers { get; set; } 
    }
}
