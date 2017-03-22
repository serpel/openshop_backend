using System.Web.Mvc;

namespace AttendanceRRHH.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: AccessDenied
        public ActionResult AccessDenied()
        {
            ViewBag.ErrorCode = 401;
            ViewBag.Description = "Access Denied";

            return View();
        }

        // GET: TODO: Display error on fancy page Error
        public ActionResult Error()
        {
            return View();
        }
    }
}