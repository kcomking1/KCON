using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using KCSystem.Core.Entities;

namespace KCSystem.Infrastructrue.Database
{
   public class DbSeed
    {
        public static async Task SeedAsync(KCDBContext db)
        {
            
            try
            {
                if (!db.Users.Any())
                { 
                     var newUser = new User
                    {
                        UserName = "Admin",
                        RealName = "Admin",
                        Phone = "18611838132",
                        Email = "492413277@qq.com",
                        AddTime = DateTime.Now, 
                        UserPassword = "AQAAAAEAACcQAAAAEAU51TeROUrGiYEVN+nNjNg/FymivZTbvIalRl3YlacQZNti432HRjXIrCPhVWDQWw=="
                        
                     };
                    db.Add(newUser);

                    var model1 = new Module
                    {
                        ModuleName="系统管理",
                        ModuleUrl="",
                        ModuleSortIndex=1,
                        ModuleDesc= "系统管理",
                        ModuleUse = true,
                        Icon= "fa fa-gear",
                        Children = new []
                        {
                            new Module()
                            {
                                ModuleName="模块管理",
                                ModuleUrl="/Modules",
                                ModuleSortIndex=1,
                                ModuleDesc= "模块管理",
                                ModuleUse = true,
                                Icon=""
                            },
                            new Module()
                            {
                                ModuleName="用户管理",
                                ModuleUrl="/Users",
                                ModuleSortIndex=2,
                                ModuleDesc= "用户管理",
                                ModuleUse = true,
                                Icon=""
                            },
                            new Module()
                            {
                                ModuleName="角色管理",
                                ModuleUrl="/Roles",
                                ModuleSortIndex=3,
                                ModuleDesc= "角色管理",
                                ModuleUse = true,
                                Icon=""
                            },
                            new Module()
                            {
                                ModuleName="字典管理",
                                ModuleUrl="/BackEnums",
                                ModuleSortIndex=4,
                                ModuleDesc= "字典管理",
                                ModuleUse = true,
                                Icon=""
                            }
                        }
                    };
                    db.Add(model1);
                    await db.SaveChangesAsync();
                }
                 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
