using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenshopBackend.Startup))]
namespace OpenshopBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
