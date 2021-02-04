using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KCSystem.Core.Entities
{
    /// <summary>
    /// 字典表
    /// </summary>
    public class BackEnum:Entity
    { 
       
        /// <summary>
        /// 字典名称
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(50, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "字典名称")]
        public String EnumName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")] 
        [Display(Name = "排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(100, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "备注")]
        public String Remark { get; set; }

        /// <summary>
        /// 父级ID（默认为0）
        /// </summary>

        [Display(Name = "上级名称")]
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public BackEnum Parent { get; set; }

        public IEnumerable<BackEnum> Children { get; set; }
        
       
        
    }
}
