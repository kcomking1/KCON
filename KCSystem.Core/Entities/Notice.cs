using System;
using System.Collections.Generic;
using System.Text;

namespace KCSystem.Core.Entities
{
   public class Notice:Entity
    {

        /// <summary>
        /// 操作用户
        /// </summary>
        public int UserId { get; set; }

        public User User { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }
    }
}
