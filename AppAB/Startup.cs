using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AppAB.Startup))]
namespace AppAB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
