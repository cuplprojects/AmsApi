using AmsApi.Models; // Ensure you have the correct namespace for your models
using Microsoft.EntityFrameworkCore;

namespace AmsApi.Data
{
    public class AMSDbContext : DbContext
    {
        public AMSDbContext(DbContextOptions<AMSDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any additional model configurations here
        }

        // Define DbSets for your entities here
        public DbSet<Asset> Assets { get; set; }
        // public DbSet<AnotherEntity> AnotherEntities { get; set; }
    }
}
