using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ImpromptuInterface;
using KCSystem.Api.AuthHelper;
using KCSystem.Api.Extensions;
using KCSystem.Api.Store;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Infrastructrue.Database;
using KCSystem.Infrastructrue.Repository;
using KCSystem.Infrastructrue.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace KCSystem.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddIdentity<User, Role>()
                .AddRoleStore<RoleStoreImp>()
                .AddUserStore<UserStoreImp>();

            //【认证】
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, CustomJwtBearerOptionsPostConfigureOptions>();
            services.AddSingleton<CustomJwtSecurityTokenHandler>();
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray); 
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;

                })// 也可以直接写字符串，AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey, //参数配置在下边
                        ValidateIssuer = true,
                        ValidIssuer = audienceConfig["Issuer"], //发行人
                        ValidateAudience = true,
                        ValidAudience = audienceConfig["Audience"], //订阅人
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true,
                    };
                    //o.SecurityTokenValidators.Clear();
                    //o.SecurityTokenValidators.Add(new CustomJwtSecurityTokenHandler());
                });
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "资管系统 API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"}
                        },new string[] { }
                    }
                });
            });


            #region CORS
            //跨域第一种方法，先注入服务，声明策略，然后再下边app中配置开启中间件
            services.AddCors(c =>
            {
                //一般采用这种方法
                c.AddPolicy("LimitRequests", policy =>
                    {
                        policy.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    });
            });

            #endregion

            services.AddControllers(op => op.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>());
            //// 注入权限处理器
            //services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddTransient<IAuthorizationPolicyProvider, ResourceAuthorizationPolicyProvider>();
            services.AddTransient<IApplicationModelProvider, ResourceApplicationModelProvider>(); 
            services.AddDbContext<KCDBContext>(p => { p.UseSqlServer(Configuration.GetConnectionString("ConnectionString")); }); 
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            
            //app.UseJsonExceptionHandler();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "资管系统 API V1");
                c.RoutePrefix = "";

            });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization(); 
            
            app.UseCors("LimitRequests");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
