using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KCSystem.Core.Entities
{
    /// <summary>
    /// 管理用户
    /// </summary>
    public class User:Entity
    {
        

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(20, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")] 
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

         

        /// <summary>
        /// 真是姓名
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(20, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "真实姓名")]
        public string RealName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")]
        [StringLength(20, ErrorMessage = "{0}不能大于{1}个字符")]
        [Display(Name = "联系电话")]
        public string Phone { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int? RoleId { get; set; }

        public Role Role { get; set; } 
        /// <summary>
        /// 上级领导
        /// </summary>
        public int? LeardId { get; set; }
        /// <summary>
        /// 上级领导
        /// </summary>
        public User Leard { get; set; } 
        /// <summary>
        /// 部门
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [ForeignKey("DepartmentId")]
        public BackEnum Department { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImage { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 电子签章
        /// </summary>
        public string Signature { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空,请填写")] 
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }


        /// <summary>
        /// 地区
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IdNo {get;set;}

        /// <summary>
        /// 身份证图片
        /// </summary>
        public string IdImage { get; set; }


        /// <summary>
        ///  座机号
        /// </summary>
        public string Telephone { get; set; }


        /// <summary>
        /// 传真号码
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 是否是办公人员
        /// </summary>
        public bool IsMember { get; set; }

        public IEnumerable<User> Children { get; set; }
        public IEnumerable<SystemLog> Logs { get; set; }
        
        
    }

    public enum Gender
    {
        Male = 0,
        Female
    }
}
