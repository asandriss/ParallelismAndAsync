using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebParallelProgrammingTutorial.Startup))]
namespace WebParallelProgrammingTutorial
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
