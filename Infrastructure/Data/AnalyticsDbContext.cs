using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
           : base(options)
        {
        }

        // Raw data from GA + PSI combined records (consumed from RabbitMQ)
        public DbSet<CombinedRecord> RawData { get; set; } = null!;

        // Aggregated daily statistics
        public DbSet<DailyStats> DailyStats { get; set; } = null!;

        // Optional: Users table if you implement JWT signup/login
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table names
            modelBuilder.Entity<CombinedRecord>().ToTable("RawData");
            modelBuilder.Entity<DailyStats>().ToTable("DailyStats");
            modelBuilder.Entity<User>().ToTable("Users");

            // Primary keys
            modelBuilder.Entity<CombinedRecord>().HasKey(r => r.Id);
            modelBuilder.Entity<DailyStats>().HasKey(d => d.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            // Column configuration examples
            modelBuilder.Entity<CombinedRecord>()
                        .Property(r => r.Page)
                        .IsRequired()
                        .HasMaxLength(200);
            modelBuilder.Entity<CombinedRecord>(entity =>
            {
                entity.Property(e => e.PerformanceScore)
                    .HasColumnType("decimal(5,4)"); // e.g., 0.1234 fits fine
            });
            modelBuilder.Entity<DailyStats>()
                        .Property(d => d.AvgPerformance)
                        .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<User>()
                        .Property(u => u.Email)
                        .IsRequired()
                        .HasMaxLength(200);
        }
    }
}
