using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebBasedInstaller.Startup))]
namespace WebBasedInstaller
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
