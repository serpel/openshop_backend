using OpenShopVHBackend.BussinessLogic;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OpenShopVHBackend.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class BaseController : Controller
    {
        // this exist because we need load cross controllers information on left or top menu
        public virtual ActionResult LeftNavBar()
        {
            return PartialView("_LeftNavBar");
        }

        public virtual ActionResult TopNavBar()
        {
            return PartialView("_TopNavBar");
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        //TODO: handle exceptions using a loger
        protected override void OnException(ExceptionContext filterContext)
        {
            MyLogger.GetInstance.Error(filterContext.Exception.Message, filterContext.Exception);
            base.OnException(filterContext);
        }

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Profile");
        }
    }
}