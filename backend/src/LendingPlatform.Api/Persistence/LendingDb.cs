using LendingPlatform.Domain;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Api.Persistence;

public class LendingDbContext : DbContext
{
    public LendingDbContext(DbContextOptions<LendingDbContext> options)
        : base(options) { }

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var app = modelBuilder.Entity<LoanApplication>();

        app.HasKey(a => a.Id);

        app.Property(a => a.LoanAmount).HasPrecision(18, 2).IsRequired();
        app.Property(a => a.AssetValue).HasPrecision(18, 2).IsRequired();
        app.Property(a => a.CreditScore).IsRequired();
        app.Property(a => a.LoanToValue).HasPrecision(9, 4).IsRequired();

        app.Property(a => a.Decision)
           .HasConversion<string>()
           .HasMaxLength(20)
           .IsRequired();

        app.Property(a => a.DeclineReason).HasMaxLength(250);

        app.Property(a => a.CreatedAtUtc)
           .HasColumnType("timestamp with time zone")
           .IsRequired();

        app.HasIndex(a => a.CreatedAtUtc);
    }
}