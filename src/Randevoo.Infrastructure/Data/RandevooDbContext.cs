
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Randevoo.Domain.Entities;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Infrastructure.Data;

public class RandevooDbContext : DbContext
{
    public RandevooDbContext(DbContextOptions<RandevooDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Interest> Interests => Set<Interest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.MobileNumber).IsRequired().HasMaxLength(20);
            b.HasIndex(u => u.MobileNumber).IsUnique();
            b.Property(u => u.Email).HasMaxLength(100);
            b.HasIndex(u => u.Email).IsUnique()
                .HasFilter("[Email] IS NOT NULL");
            b.Property(u => u.PendingEmail).HasMaxLength(100);
            b.Property(u => u.MobileLoginCodeHash).HasMaxLength(128);
            b.Property(u => u.EmailConfirmationTokenHash).HasMaxLength(128);
            b.HasQueryFilter(u => !u.IsDeleted);
            b.HasOne(u => u.Profile)
             .WithOne(p => p.User)
             .HasForeignKey<UserProfile>(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // UserProfile
        modelBuilder.Entity<UserProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.DisplayName).IsRequired().HasMaxLength(50);
            b.HasIndex(p => p.DisplayName).IsUnique();
            b.HasIndex(p => p.UserId).IsUnique();
            b.HasQueryFilter(p => !p.IsDeleted);
            b.OwnsOne(p => p.Height, hb =>
            {
                hb.Property(h => h.Centimeters)
                    .HasColumnName("HeightCentimeters")
                    .IsRequired();
            });

            b.OwnsOne(p => p.Location, lb =>
            {
                lb.Property(l => l.Country).HasColumnName("Location_Country").HasMaxLength(100).IsRequired();
                lb.Property(l => l.City).HasColumnName("Location_City").HasMaxLength(100).IsRequired();
                lb.Property(l => l.Region).HasColumnName("Location_Region").HasMaxLength(100);
                lb.OwnsOne(l => l.Coordinates, cb =>
                {
                    cb.Property(c => c.Latitude).HasColumnName("Location_Latitude").HasPrecision(9, 6).IsRequired();
                    cb.Property(c => c.Longitude).HasColumnName("Location_Longitude").HasPrecision(9, 6).IsRequired();
                });
            });

            b.HasMany(p => p.Interests)
             .WithMany(i => i.UserProfiles)
             .UsingEntity<Dictionary<string, object>>(
                 "UserProfileInterest",
                 r => r.HasOne<Interest>().WithMany().HasForeignKey("InterestId").OnDelete(DeleteBehavior.Cascade),
                 l => l.HasOne<UserProfile>().WithMany().HasForeignKey("UserProfileId").OnDelete(DeleteBehavior.Cascade),
                 j =>
                 {
                     j.HasKey("UserProfileId", "InterestId");
                     j.ToTable("UserProfileInterests");
                 });

            b.Navigation(p => p.Interests).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // Interest
        modelBuilder.Entity<Interest>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.Name).IsRequired().HasMaxLength(50);
            b.HasIndex(i => i.Name).IsUnique();
            b.Property(i => i.Category).HasMaxLength(30);
            b.Property(i => i.UsageCount).IsRequired();
            b.HasQueryFilter(i => !i.IsDeleted);
        });
    }
}
