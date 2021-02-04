using KCSystem.Core.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KCSystem.Web.Models;

namespace KCSystem.Web.Extensions
{
    public class EnumHelper
    {
        /// <summary>
        /// 获取自定义属性获取的内容
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Object obj)
        {

            //获取枚举对象的枚举类型
            Type type = obj.GetType();
            //通过反射获取该枚举类型的所有属性
            FieldInfo[] fieldInfos = type.GetFields();

            foreach (FieldInfo field in fieldInfos)
            {

                //不是参数obj,就直接跳过

                if (field.Name != obj.ToString())
                {
                    continue;
                }

                //取出参数obj的自定义属性
                if (field.IsDefined(typeof(SelectDisplayNameAttribute), true))
                {

                    return (field.GetCustomAttributes(typeof(SelectDisplayNameAttribute), true)[0] as SelectDisplayNameAttribute).DisplayName;
                }

            }

            return obj.ToString();

        }

        /// <summary>
        ///  将枚举类型的值和自定义属性配对组合为 List<SelectListItem>
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetSelectList<T>()
        {
            var enumType = typeof(T);
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var obj in Enum.GetValues(enumType))
            {
                // 注意一定要Value = obj.ToString() 
                //原因是Value = (int)(obj.ToString()),这样取的枚举是对应的int值,在@Html.DropDownListFor中是无法显示默认选中值的,问题原因未解
                selectList.Add(new SelectListItem { Text = GetEnumDescription(obj), Value = obj.ToString() });
            }
            return selectList;

        }

        /// <summary>
        /// 客户信息分析管理 导入文件选择枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string  GetEnum<T>(string name)
        {
            var str = string.Empty;
            var enumType = typeof(T);     
            foreach (var obj in Enum.GetValues(enumType))
            {
                var text = GetEnumDescription(obj);
                if (name.Replace("万","") == text.Replace("万", ""))
                {
                    str = obj.ToString();
                }
            }
            return str;

        }

        
    }

}
