using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace KCSystem.Api.AuthHelper
{
    /// <summary>
    /// JWT Option 设置 添加自定义验证
    /// </summary>
    public class CustomJwtBearerOptionsPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly CustomJwtSecurityTokenHandler _tokenValidator; //example dependancy
        /// <summary>
        /// CustomJwtBearerOptionsPostConfigureOptions
        /// </summary>
        /// <param name="tokenValidator">自定义token 验证</param>
        public CustomJwtBearerOptionsPostConfigureOptions(CustomJwtSecurityTokenHandler tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }
        /// <summary>
        /// Option修改添加自定义验证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void PostConfigure(string name, JwtBearerOptions options)
        {
            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(_tokenValidator);
        }
    }
}
