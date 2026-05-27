
using Microsoft.EntityFrameworkCore;
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
            b.Property(u => u.Email).IsRequired().HasMaxLength(200);
            b.Property(u => u.PasswordHash).IsRequired();
            b.HasOne(u => u.Profile)
             .WithOne(p => p.User)
             .HasForeignKey<UserProfile>(p => p.UserId);
        });

        // UserProfile
        modelBuilder.Entity<UserProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.DisplayName).IsRequired().HasMaxLength(50);
            b.OwnsOne(typeof(Height), "Height", hb =>
            {
                // If Height is a value object class, map its Centimeters property (adjust name if needed)
                hb.Property<int>("Centimeters").HasColumnName("HeightCentimeters");
            });

            // Location is a value object: store as owned
            b.OwnsOne(typeof(Location), "Location", lb =>
            {
                lb.Property<string>("Country").HasColumnName("Location_Country").HasMaxLength(100);
                lb.Property<string>("City").HasColumnName("Location_City").HasMaxLength(100);
                // Coordinates inside Location
                lb.OwnsOne(typeof(Coordinates), "Coordinates", cb =>
                {
                    cb.Property<decimal>("Latitude").HasColumnName("Location_Latitude");
                    cb.Property<decimal>("Longitude").HasColumnName("Location_Longitude");
                });
            });

            // Many-to-many between UserProfile and Interest (EF Core 5+ implicit join)
            b.HasMany(typeof(Interest), "_interests")
             .WithMany("UserProfiles");
        });

        // Interest
        modelBuilder.Entity<Interest>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.Name).IsRequired().HasMaxLength(100);
            b.Property(i => i.Category).HasMaxLength(100);
            b.Property(i => i.UsageCount).IsRequired();
        });
    }
}