using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCSystem.Core.Entities
{
    /// <summary>
    /// 模块
    /// </summary>
   public class Module: Entity
    {

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(50, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 权限URL
        /// </summary>
        
        [StringLength(50, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "地址")]
        public string ModuleUrl { get; set; }

        /// <summary>
        /// 显示排序
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        
        [Display(Name = "排序")]
        public int ModuleSortIndex { get; set; }

        /// <summary>
        /// 权限说明
        /// </summary>
        public string ModuleDesc { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public Boolean ModuleUse { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 上级
        /// </summary>
        public Module Parent { get; set; }

        public IEnumerable<Module> Children { get; set; }

         
    }
}
