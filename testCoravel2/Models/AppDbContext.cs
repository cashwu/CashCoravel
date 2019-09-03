using Coravel.Pro.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace testCoravel2.Models
{
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