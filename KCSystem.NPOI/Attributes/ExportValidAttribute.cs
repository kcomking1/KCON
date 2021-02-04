using System;
using System.Collections.Generic;
using System.Text;

namespace NPOI.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class  ExportValidAttribute:Attribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get;set;
        }

        /// <summary>
        /// 位置索引
        /// </summary>
        public int Index
        {
            get;set;
        }
        /// <summary>
        /// 公式
        /// </summary>
        public string Formula
        {
            get; set;
        }
        /// <summary>
        /// 验证日期
        /// </summary>
        public ValidType ValidType { get; set; } = 0; 
    }

    public enum ValidType
    {
        String=0,
        Integer,
        Date,
        DropDown,
        Phone,
        Formula
    }
}
