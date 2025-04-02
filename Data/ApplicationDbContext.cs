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
    //public DbSet<Event> Events { get; set; }
    
    //public DbSet<Models.User> AppUsers { get; set; }
    
}