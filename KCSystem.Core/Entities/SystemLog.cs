using System;
using System.Collections.Generic;
using System.Text;

namespace KCSystem.Core.Entities
{
    /// <summary>
    ///  系统日志
    /// </summary>
    public class SystemLog : Entity
    {
        /// <summary>
        /// 操作用户
        /// </summary>
        public int UserId { get; set; }
        
        public User User { get; set; }

        public string Scenarios { get; set; }
        public string Description { get; set; }
    }
}
