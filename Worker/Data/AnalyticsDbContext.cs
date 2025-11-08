using Consumer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
            : base(options)
        {
        }

        // Raw data from GA + PSI combined records
        public DbSet<CombinedRecord> RawData { get; set; } = null!;

        // Aggregated daily statistics
        public DbSet<DailyStats> DailyStats { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CombinedRecord>().ToTable("RawData");
            modelBuilder.Entity<DailyStats>().ToTable("DailyStats");


            modelBuilder.Entity<CombinedRecord>()
                        .Property(r => r.Page)
                        .IsRequired()
                        .HasMaxLength(200);

            modelBuilder.Entity<DailyStats>()
                        .Property(d => d.AvgPerformance)
                        .HasColumnType("decimal(5,2)");
        }
    }
}
