using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace OpenshopBackend.BussinessLogic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool isAuthenticated = filterContext.HttpContext.Request.IsAuthenticated;

            if (isAuthenticated) {

                var user = filterContext.HttpContext.User;
                string [] roles = Roles.Split(',');

                bool isInRole = false;
                foreach(var item in roles)
                {
                    if (user.IsInRole(item.Trim()))
                    {
                        isInRole = true;
                        break;
                    }
                }

                if (!isInRole)
                {
                    MyLogger.GetInstance.Info("Access denied on controller "+filterContext.Controller);

                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }
            }
        }
    }
}