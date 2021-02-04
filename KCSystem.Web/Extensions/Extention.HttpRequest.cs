using Microsoft.AspNetCore.Http;

namespace KCSystem.Web.Extensions
{
    /// <summary>
    /// 拓展类
    /// </summary>
    public static partial class Extention
    {
        public static bool IsAjaxRequest(this HttpRequest req)
        {
            bool result = false;

            var xreq = req.Headers.Get("x-requested-with");
            if (!string.IsNullOrEmpty( xreq))
            {
                result = req.Headers["x-requested-with"] == "XMLHttpRequest";
            }

            return result;
        }
    }
}
