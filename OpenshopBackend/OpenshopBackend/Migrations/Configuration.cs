namespace OpenshopBackend.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OpenshopBackend.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "OpenshopBackend.Models.ApplicationDbContext";
        }

        protected override void Seed(OpenshopBackend.Models.ApplicationDbContext context)
        {
            string email = "sergio.peralta@kattangroup.com";

            //create the first user
            if (!(context.Users.Any(u => u.Email == email)))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser { UserName = email, Email = email };
                var result = userManager.Create(userToInsert, "Admin.1234");

                //create and asign roles
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);
                    var adminRole = new IdentityRole("Admin");
                    result = roleManager.Create(adminRole);

                    if (result.Succeeded)
                    {
                        userManager.AddToRoles(userToInsert.Id, adminRole.Name);
                    }
                }
            }

            //var categoy = new Category()
            //{
            //    Id = -1,
            //    Name = "Menu",
            //    PartentId = 0,
            //    RemoteId = -1,
            //    Type = "category"
            //};

            //var category2 = new Category()
            //{
            //    Id = -2,
            //    Name = "Clientes",
            //    PartentId = 0,
            //    RemoteId = -2,
            //    Type = "category"
            //};

            //context.Categories.Add(categoy);
            //context.Categories.Add(category2);
            //context.SaveChanges();
        }
    }
}
