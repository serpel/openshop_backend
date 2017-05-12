using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using OpenShopVHBackend.BussinessLogic;
using OpenShopVHBackend.Models;
using OpenShopVHBackend.Properties;

namespace OpenShopVHBackend.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin")]
    public class UsersController: BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUsers()
        {

            var result = db.Users
                        .Select(s => new { s.Id, s.UserName, s.Email, s.EmailConfirmed });

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(string id)
        {
            var user = UserManager.FindById(id);

            string[] selectedRoles = user.Roles.Select(x => x.RoleId).ToArray();

            ViewBag.Roles = new MultiSelectList(db.Roles.ToList(), "Id", "Name", null, selectedRoles);
            UserViewModel userV = new UserViewModel() { Id = user.Id, Email = user.Email, ProfileUrl = user.ProfileUrl };

            return PartialView("_Edit", userV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model, HttpPostedFileBase file)
        {
            //TODO: Fix error when delete all companies
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await UserManager.FindByIdAsync(model.Id);

                    if (file != null && file.ContentLength > 0)
                    {
                        string baseUrl = Url.Content(Path.Combine(@ConfigurationManager.AppSettings["ProfileImagePath"], model.Email));
                        string path = Uploader.GetInstance.GenerateUrlPath(Server.MapPath(baseUrl), baseUrl, file);
                        user.ProfileUrl = Url.Content(path);
                    }
                    
                    if(model.Password != null)
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
                    }

                    await UserManager.UpdateAsync(user);

                    var roleStore = new RoleStore<IdentityRole>(db);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var rolesToDelete = user.Roles.Select(s => s.RoleId).Except(model.Roles).ToList();
                    var rolesToAdd = model.Roles.Except(user.Roles.Select(s => s.RoleId)).ToList();

                    if (rolesToDelete.Count > 0)
                    {
                        string[] roles = db.Roles
                            .Where(w => rolesToDelete.Contains(w.Id))
                            .Select(s => s.Name)
                            .ToArray();

                        await UserManager.RemoveFromRolesAsync(user.Id, roles);
                    }

                    if (rolesToAdd.Count > 0)
                    {
                        string[] newRoles = db.Roles
                            .Where(w => rolesToAdd.Contains(w.Id))
                            .Select(s => s.Name)
                            .ToArray();

                        await UserManager.AddToRolesAsync(user.Id, newRoles);
                    }
                }
                catch (Exception e)
                {
                    MyLogger.GetInstance.Error("Error ", e);

                }

                //TODO: save company user
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.Roles = new SelectList(db.Roles.ToList(), "Id", "Name");
            return PartialView("_Edit", model);
        }

        public ActionResult Delete(string id)
        {
            var user = UserManager.FindById(id);
            UserViewModel userV = new UserViewModel() { Id = user.Id, Email = user.Email };

            return PartialView("_Delete", userV);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(UserViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            var logins = user.Logins;

            foreach (var login in logins.ToList())
            {
                await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }

            var rolesForUser = await UserManager.GetRolesAsync(user.Id);

            foreach (var item in rolesForUser.ToList())
            {
                await UserManager.RemoveFromRoleAsync(user.Id, item);
            }

            var userClaims = await UserManager.GetClaimsAsync(user.Id);

            foreach (var item in userClaims.ToList())
            {
                await UserManager.RemoveClaimAsync(user.Id, item);
            }

            var result = await UserManager.DeleteAsync(user);

            MyLogger.GetInstance.Info("User was delted Succesfull, userId: " + user.Id + " Email: " + user.Email);

            return Json(new { success = result.Succeeded }, JsonRequestBehavior.AllowGet);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //
        // GET: /Account/Register
        public ActionResult Create()
        {
            return PartialView("_Create", new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if(file != null && file.ContentLength > 0)
                {
                    string baseUrl = Url.Content(Path.Combine(@ConfigurationManager.AppSettings["ProfileImagePath"], model.Email));
                    string path = Uploader.GetInstance.GenerateUrlPath(Server.MapPath(baseUrl), baseUrl, file);
                    model.ProfileUrl = Url.Content(path);
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, ProfileUrl = model.ProfileUrl };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, Resources.ConfirmYourAccount, Resources.ConfirmYourAccountMessage + " <a href=\"" + callbackUrl + "\">"+ Resources.Here +"</a>");

                    MyLogger.GetInstance.Info("User was created Succesfull, userId: " + user.Id + " Email: " + user.Email);

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return PartialView("_Create", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}