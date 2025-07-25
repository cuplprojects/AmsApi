 // Ensure you have the correct namespace for your models
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AmsApi.Models;
using AmsApi.Model;



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
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssetRequest> AssetRequests { get; set; }
        public DbSet<TransferDetails> TransferDetails { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SellOrDispose> SellOrDisposes { get; set; }
        public DbSet<AssetDocument> AssetDocuments { get; set; }
        public DbSet<EventLogs> eventLogs { get; set; }
        public DbSet<ErrorLog> errorLogs { get; set; }
        public DbSet<AssetStatus> assetStatus { get; set; }
        public DbSet<AssetCategory> assetCategories { get; set; }

        // public DbSet<AnotherEntity> AnotherEntities { get; set; }
    }
}
