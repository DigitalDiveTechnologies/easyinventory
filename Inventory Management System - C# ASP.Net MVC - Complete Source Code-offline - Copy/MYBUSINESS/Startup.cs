using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MYBUSINESS.Startup))]

namespace MYBUSINESS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
