using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(e_ptit_2.Startup))]
namespace e_ptit_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
