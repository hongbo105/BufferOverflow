using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyFixIt3.Startup))]
namespace MyFixIt3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
