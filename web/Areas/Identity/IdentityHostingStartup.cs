using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(web.Areas.Identity.IdentityHostingStartup))]
namespace web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}