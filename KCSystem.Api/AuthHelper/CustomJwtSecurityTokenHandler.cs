using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Dynamitey.DynamicObjects;
using KCSystem.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens; 

namespace KCSystem.Api.AuthHelper
{
    /// <summary>
    /// 自定义token验证
    /// </summary>
    public class CustomJwtSecurityTokenHandler : ISecurityTokenValidator
    {
        private int _maxTokenSizeInBytes = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 自定义token验证
        /// </summary>
        /// <param name="cache"></param>
        public CustomJwtSecurityTokenHandler(IMemoryCache cache)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _cache = cache;
        }
        /// <summary>
        /// 验证
        /// </summary>
        public bool CanValidateToken
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MaximumTokenSizeInBytes
        {
            get
            {
                return _maxTokenSizeInBytes;
            }

            set
            {
                _maxTokenSizeInBytes = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="securityToken"></param>
        /// <returns></returns>
        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <param name="validatedToken"></param>
        /// <returns></returns>
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //How to access HttpContext/IP address from here? 
            
            var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);

            //给Identity赋值
            var identity = new ClaimsIdentity();
            identity.AddClaims(principal.Claims);

            List<Claim> claims;
            _cache.TryGetValue(principal.ExtractUserId().Value, out claims);
            if(claims != null)
                identity.AddClaims(claims);

            var principle = new ClaimsPrincipal(identity);
            return principle; 
        }
    }
}