using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KCSystem.Api.AuthHelper
{
    public class ResourceApplicationModelProvider : IApplicationModelProvider
    {
        private readonly IAuthorizationPolicyProvider _policyProvider;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public ResourceApplicationModelProvider(IAuthorizationPolicyProvider policyProvider)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            _policyProvider = policyProvider;
        }
        

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
           
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            List<ResourceAttribute> attributeData = new List<ResourceAttribute>();
            foreach (var controllerModel in context.Result.Controllers)
            {
                var resourceData = controllerModel.Attributes.OfType<ResourceAttribute>().ToArray();
                if (resourceData.Length > 0)
                {
                    attributeData.AddRange(resourceData);
                }

                foreach (var actionModel in controllerModel.Actions)
                {
                    var actionResourceData = actionModel.Attributes.OfType<ResourceAttribute>().ToArray();
                    if (actionResourceData.Length > 0)
                    {
                        attributeData.AddRange(actionResourceData);
                    }
                }
            }

            foreach (var item in attributeData)
            {
                ResourceData.AddResource(item.GetResource(), item.Action??"");
            }
        }

        public int Order { get { return -1000 + 11; } }
    }
}
