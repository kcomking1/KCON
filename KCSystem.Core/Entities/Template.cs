using System;
using System.Collections.Generic;
using System.Text;

namespace KCSystem.Core.Entities
{
    /// <summary>
    /// 项目关联模板管理
    /// </summary>
   public class Template:Entity
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public int ProId { get; set; }

      

        /// <summary>
        /// 模板Id
        /// </summary>
        public int TemplateId { get; set; }

        public DocumentTemplate DocumentTemplate { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public bool Audit { get; set; }
    }
}
