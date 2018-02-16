using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GAPT.Startup))]
namespace GAPT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
