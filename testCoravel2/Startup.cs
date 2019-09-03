using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel;
using Coravel.Pro;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using testCoravel2.Models;

namespace testCoravel2
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
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), optionsBuilder =>
                {
                    optionsBuilder.CommandTimeout(30);
                    optionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), new List<string>());
                });
                options.EnableSensitiveDataLogging();
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCoravelPro(typeof(AppDbContext));

            services.AddScoped<MyJob>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseCoravelPro();

            var provider = app.ApplicationServices;

            provider.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<MyJob>()
                    .EveryMinute();

            }).LogScheduledTaskProgress(provider.GetService<ILogger<IScheduler>>());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}