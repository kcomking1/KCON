using System;
using Microsoft.AspNetCore.Authorization;

namespace KCSystem.Web.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited =true)]
    public class ResourceAttribute:AuthorizeAttribute
    {
        private string _resouceName;
        private string _action;
        public ResourceAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            _resouceName = name;
            
            Policy = _resouceName;
        }

        public string GetResource()
        {
            return _resouceName;
        }
        public string Action
        {
            get => _action;
            set
            {
                _action = value ?? "";
                if (!string.IsNullOrEmpty(value))
                {
                    Policy = _resouceName + "-" + value;
                }
            }
        }
    }
}
