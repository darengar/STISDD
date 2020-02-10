using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EMR.Startup))]
namespace EMR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
