using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Facturador.Startup))]
namespace Facturador
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
