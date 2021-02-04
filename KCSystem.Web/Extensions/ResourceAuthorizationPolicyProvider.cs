using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KCSystem.Web.Extensions
{
    public class ResourceAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private  AuthorizationOptions _options;
        public ResourceAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(_options.DefaultPolicy);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult(_options.FallbackPolicy);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy policy = _options.GetPolicy(policyName);
            if (policy == null)
            {
                string[] resourceValues = policyName.Split(new char[] { '-' }, StringSplitOptions.None);
                if (resourceValues.Length == 1)
                {
                    _options.AddPolicy(policyName, builder =>
                    {
                        builder.AddRequirements(new ClaimsAuthorizationRequirement(resourceValues[0], new string[] {"" }));
                    });
                }
                else
                {
                    _options.AddPolicy(policyName, builder =>
                    {
                        builder.AddRequirements(new ClaimsAuthorizationRequirement(resourceValues[0], new string[] { resourceValues[1] }));
                    });
                }
            }
            return Task.FromResult(_options.GetPolicy(policyName));
        }
    }
}
