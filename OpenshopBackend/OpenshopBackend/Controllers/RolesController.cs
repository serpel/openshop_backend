using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using OpenshopBackend.BussinessLogic;
using OpenshopBackend.Models;

namespace OpenshopBackend.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin")]
    public class RolesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRoles()
        {
            var result = db.Roles
                .Select(s => new { s.Id, s.Name });

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var role = new IdentityRole();
            return PartialView("Create", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();

                MyLogger.GetInstance.Info("Role was created successfull: " + role.Name);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("Create", role);
        }

        public ActionResult Edit(string id)
        {
            var role = db.Roles.Find(id);
            return PartialView("Edit", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                MyLogger.GetInstance.Info("Role was edited successfull: " + role.Name);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("Edit", role);
        }

        // GET: ShiftTimes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = db.Roles.Find(id);

            if (role == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            IdentityRole role = db.Roles.Find(id);
            db.Roles.Remove(role);
            db.SaveChanges();

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}