﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KCSystem.Web.Extensions
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
       
        
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return httpContext.User.Identity.IsAuthenticated;
            }
        
    }
}
