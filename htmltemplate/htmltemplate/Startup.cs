using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(htmltemplate.Startup))]
namespace htmltemplate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
