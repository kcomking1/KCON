using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KCSystem.Core.Entities
{
    /// <summary>
    /// 文档模板
    /// </summary>
   public class DocumentTemplate : Entity
    {

        /// <summary>
        /// 操作用户
        /// </summary>
        public int UserId { get; set; }

        public User User { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 模板地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 模板类型
        /// </summary>
        [ForeignKey("TypeId")]
        public BackEnum Type { get; set; }
    }
}
