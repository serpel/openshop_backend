using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenshopBackend.Startup))]
namespace OpenshopBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            ConfigureAuth(app);
        }
    }
}
