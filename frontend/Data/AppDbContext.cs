using DataAnalizer.Models;
using Microsoft.EntityFrameworkCore;

using System.IO;

namespace DataAnalizer.Data;

public class AppDbContext : DbContext
{
    public DbSet<AnalysisRecord> Analyses => Set<AnalysisRecord>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        string appFolder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "RealEstateDataPlatform"
        );

        Directory.CreateDirectory(appFolder);

        string databasePath = Path.Combine(
            appFolder,
            "realestate.db"
        );

        optionsBuilder.UseSqlite(
            $"Data Source={databasePath}"
        );
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisRecord>()
            .HasIndex(x => x.BackendDatasetId)
            .IsUnique();
    }
}