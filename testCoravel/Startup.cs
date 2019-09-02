using Coravel;
using Coravel.Pro;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
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
            services.AddScoped<MyJob>();

            services.AddScheduler();
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

            provider.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<MyJob>()
                    .EveryMinute();
            }).LogScheduledTaskProgress(provider.GetService<ILogger<IScheduler>>());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}