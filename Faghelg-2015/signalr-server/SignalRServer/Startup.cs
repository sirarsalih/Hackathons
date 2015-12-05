using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRServer.Startup))]
namespace SignalRServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
