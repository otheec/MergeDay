using MergeDay.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Infrastructure.Persistence;

public class MergeDayDbContext(DbContextOptions<MergeDayDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Absence> Absences { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<TogglProject> TogglProjects { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillItem> BillItems { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bill>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.IBAN).IsRequired();
            e.Property(x => x.Total).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.ApplicationUser)
             .WithMany()
             .HasForeignKey(x => x.ApplicationUserId)
             .OnDelete(DeleteBehavior.NoAction);

            e.HasMany(x => x.Items)
             .WithOne(i => i.Bill)
             .HasForeignKey(i => i.BillId)
             .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.ApplicationUserId);
        });

        modelBuilder.Entity<BillItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.ApplicationUser)
             .WithMany()
             .HasForeignKey(x => x.ApplicationUserId)
             .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.BillId);
            e.HasIndex(x => x.ApplicationUserId);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.TokenHash).IsRequired().HasMaxLength(256);
            e.Property(x => x.ReplacedByTokenHash).HasMaxLength(256);

            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.ApplicationUserId);

            e.HasOne(x => x.ApplicationUser)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(x => x.ApplicationUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
