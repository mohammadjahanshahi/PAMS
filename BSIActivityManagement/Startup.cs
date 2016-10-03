using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BSIActivityManagement.Startup))]
namespace BSIActivityManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
