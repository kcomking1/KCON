using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSystem.Infrastructrue.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace KCSystem.Api
{
    public class Program
    {
       
            public static void Main(string[] args)
            {

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var host = CreateWebHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                    try
                    {
                        // �� system.IServicec�ṩ�����ȡ T ���͵ķ���
                        var myContext = services.GetRequiredService<KCDBContext>();
                        if (myContext.Database.GetPendingMigrations().Any())
                        {
                            myContext.Database.Migrate(); //ִ��Ǩ��
                        }
                        DbSeed.SeedAsync(myContext).Wait();
                    }
                    catch (Exception e)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(e, "Error occured seeding the Database.");
                    }
                }
                host.Run();

            }

            public static IHostBuilder CreateWebHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    }).UseSerilog((context, configure) =>
                    {
                        configure.ReadFrom.Configuration(context.Configuration);
                    });

        }
    
}
