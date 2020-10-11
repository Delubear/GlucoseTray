using Dexcom.Fetch.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace GlucoseTrayCore.Data
{
    public class SQLiteDbContext : DbContext
    {
        public DbSet<GlucoseResult> GlucoseResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=" + Constants.DatabaseLocation);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GlucoseResult>().Property(e => e.Source).HasConversion(v => v.ToString(), v => (FetchMethod)Enum.Parse(typeof(FetchMethod), v));
            modelBuilder.Entity<GlucoseResult>().Property(e => e.Trend).HasConversion(v => v.ToString(), v => (TrendResult)Enum.Parse(typeof(TrendResult), v));
            modelBuilder.Entity<GlucoseResult>().Property(e => e.MmolValue).HasColumnType("decimal(3,2)");
            base.OnModelCreating(modelBuilder);
        }
    }
}
