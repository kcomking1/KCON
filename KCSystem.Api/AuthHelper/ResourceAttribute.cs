using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KCSystem.Api.AuthHelper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited =true)]
    public class ResourceAttribute:AuthorizeAttribute
    {
        private string _resouceName;
        private string _action;
        public ResourceAttribute(string name = "默认")
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
