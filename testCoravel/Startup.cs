using Coravel;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using testCoravel.Models;

namespace testCoravel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddEvents();
            services.AddQueue();
            
            services.AddScoped<NotifyNewPost>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            var provider = app.ApplicationServices;
            
            provider.ConfigureQueue()
                .LogQueuedTaskProgress(provider.GetService<ILogger<IQueue>>());
            
            var registration = provider.ConfigureEvents();

            registration
                .Register<BlogPostCreated>()
                .Subscribe<NotifyNewPost>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}