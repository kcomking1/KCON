using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KCSystem.Core.Entities;
using KCSystem.Web.Extensions;
using KCSystem.Web.Store;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using KCSystem.Infrastructrue.Database;
using KCSystem.Core.Interface;
using KCSystem.Infrastructrue.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;

namespace KCSystem.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthorization();
            services.AddAuthentication();

             services.AddIdentity<User, Role>()
             .AddRoleStore<RoleStoreImp>()
             .AddUserStore<UserStoreImp>()
             .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
             .AddDefaultTokenProviders();
             services.ConfigureApplicationCookie(option =>
            {
                //系统默认无指定 Authorize 跳转登录
                option.LoginPath = new PathString("/Signin"); //设置登陆失败或者未登录授权的情况下，直接跳转的路径这里
                option.AccessDeniedPath = new PathString("/AccessDenied"); //没有权限时的跳转页面

                //设置cookie只读情况
                option.Cookie.HttpOnly = true;
                option.SlidingExpiration = true; 
                //cookie过期时间
                //option.Cookie.Expiration = TimeSpan.FromSeconds(10);//此属性已经过期忽略，使用下面的设置
                option.ExpireTimeSpan = new TimeSpan(12, 0, 0);// 
                option.SessionStore = new MemoryCacheTicketStore();
            });



            
            //services.AddAutoMapper();
            services.AddControllersWithViews(options =>
                options.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>()).AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; 
                //不使用驼峰样式的key
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //options.SerializerSettings.DateFormatString = "yyyy年MM月dd日 HH:mm:ss";
            }).AddRazorRuntimeCompilation(); ;


            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MemoryBufferThreshold = int.MaxValue;
            });


            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });

            services.AddDbContext<KCDBContext>(p =>
            {
                p.UseSqlServer(Configuration.GetConnectionString("ConnectionString"));
            });
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(DropdownHelper));
            //注册策略提供程序
            services.AddTransient<IAuthorizationPolicyProvider, ResourceAuthorizationPolicyProvider>();
            services.AddTransient<IApplicationModelProvider, ResourceApplicationModelProvider>();

            //services.AddSingleton(Configuration);
            //services.AddTransient<ITypeHelperService, TypeHelperService>();


            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("ConnectionString")));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            } 

            app.UseStaticFiles(); 
            app.UseSerilogRequestLogging(); 
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            //RecurringJob.AddOrUpdate<AssetVerificationAutoSearch>("AssetVerificationAutoSearch", a =>
            //        a.Execute(0)
            //    , Cron.Daily);
            //RecurringJob.AddOrUpdate<ExcuteinfoReptileAutoSearch>("ExcuteinfoReptileAutoSearch", a =>
            //        a.Execute()
            //    , Cron.Weekly);
            //RecurringJob.AddOrUpdate<JudgmentDocumentAutoSearch > ("JudgmentDocumentAutoSearch", a =>
            //       a.Execute()
            //   , Cron.Weekly);
            //RecurringJob.AddOrUpdate<UpdateOverdueDays>("UpdateOverdueDays", a =>
            //        a.Execute()
            //    , Cron.Daily);
            //RecurringJob.AddOrUpdate<LiquidateCaseCheck>("LiquidateCaseCheck", a =>
            //        a.Check()
            //    , "0 0 1,13 * * ?", TimeZoneInfo.Local);//每天执行两次，每天1时和13时各执行一次 
            //RecurringJob.AddOrUpdate<InsertRepaymentMessage>("InsertRepaymentMessage", a =>
            //        a.Execute()
            //    , "0 24 2 24 * ?");
            //RecurringJob.AddOrUpdate<PersonnelNotice>("PersonnelNotice", a =>
            //        a.Execute()
            //    , Cron.Daily);
            //RecurringJob.AddOrUpdate<BidInfoAutoSearch>("BidInfoAutoSearch", a =>
            //        a.Execute()
            //    , Cron.Daily,TimeZoneInfo.Local);
            //RecurringJob.AddOrUpdate<StandardActionCheck>("StandardActionCheck", a =>
            //       a.Execute()
            //   , "0 15 1 L * ?");//每月最后一天执行1:15执行
        }
    }
}
