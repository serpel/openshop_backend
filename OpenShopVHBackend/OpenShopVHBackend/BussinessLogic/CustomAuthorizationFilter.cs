using Hangfire.Dashboard;
using Hangfire.Annotations;
using System.Collections.Generic;
using Microsoft.Owin;
using System;
using System.Web;

namespace OpenShopVHBackend.BussinessLogic
{
    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize([NotNull] IDictionary<string, object> owinEnvironment)
        {
            var context = new OwinContext(owinEnvironment);
            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return context.Authentication.User.Identity.IsAuthenticated;
        }
    }
}