using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LivogRøre.Models;
using Microsoft.AspNetCore.Identity;

namespace LivogRøre.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> AppUsers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Location relationships
        modelBuilder.Entity<Location>()
            .HasMany(l => l.Users)
            .WithOne(u => u.PreferredLocation)
            .HasForeignKey(u => u.PreferredLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Location>()
            .HasMany(l => l.Events)
            .WithOne(e => e.Location)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed initial locations for Sørlandet
        modelBuilder.Entity<Location>().HasData(
            new Location { Id = 1, Name = "Kristiansand", County = "Agder" },
            new Location { Id = 2, Name = "Arendal", County = "Agder" },
            new Location { Id = 3, Name = "Grimstad", County = "Agder" },
            new Location { Id = 4, Name = "Mandal", County = "Agder" },
            new Location { Id = 5, Name = "Farsund", County = "Agder" },
            new Location { Id = 6, Name = "Flekkefjord", County = "Agder" }
        );
    }
}