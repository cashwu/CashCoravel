using System;
using System.Collections.Generic;
using Coravel;
using Coravel.Pro;
using Coravel.Pro.EntityFramework;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), optionsBuilder =>
                {
                    optionsBuilder.CommandTimeout(30);
                    optionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), new List<string>());
                });
                options.EnableSensitiveDataLogging();
            });
            services.AddControllersWithViews();

//            services.AddEvents();
//            services.AddQueue();
//            services.AddScheduler();

            services.AddScoped<NotifyNewPost>();
            services.AddScoped<MyJob>();

            services.AddCoravelPro(typeof(AppDbContext));
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

            app.UseCoravelPro();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class AppDbContext : DbContext, ICoravelProDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CoravelJobHistory> Coravel_JobHistory { get; set; }
        public DbSet<CoravelScheduledJob> Coravel_ScheduledJobs { get; set; }
        public DbSet<CoravelScheduledJobHistory> Coravel_ScheduledJobHistory { get; set; }
    }
}