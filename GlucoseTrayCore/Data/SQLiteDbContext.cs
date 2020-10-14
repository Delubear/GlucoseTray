using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace GlucoseTrayCore.Data
{
    public interface IGlucoseTrayDbContext
    {
        DbSet<GlucoseResult> GlucoseResults { get; set; }
        int SaveChanges();
        DatabaseFacade Database { get; }
    }

    public class SQLiteDbContext : DbContext, IGlucoseTrayDbContext
    {
        public SQLiteDbContext(DbContextOptions<SQLiteDbContext> options) : base(options)
        {
        }

        public DbSet<GlucoseResult> GlucoseResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GlucoseResult>().Property(e => e.Source).HasConversion(v => v.ToString(), v => (FetchMethod)Enum.Parse(typeof(FetchMethod), v));
            modelBuilder.Entity<GlucoseResult>().Property(e => e.Trend).HasConversion(v => v.ToString(), v => (TrendResult)Enum.Parse(typeof(TrendResult), v));
            modelBuilder.Entity<GlucoseResult>().Property(e => e.MmolValue).HasColumnType("decimal(3,2)");
            base.OnModelCreating(modelBuilder);
        }
    }
}
