using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LivogRøre.Data;
using LivogRøre.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Call the database initializer
using (var services = app.Services.CreateScope())
{
    var db = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = services.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Initializing database...");
        ApplicationDbInitializer.Initialize(db);
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// Create roles and the admin user at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    string[] roles = new[] { "Admin", "User", "Company" };

    // Ensure all roles exist
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            logger.LogInformation("Created role: {Role}", role);
        }
    }

    // Create default admin user
    string adminEmail = "admin@gmail.com";
    string adminPassword = "Password!1";
    
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation("Created admin user: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create admin user: {Errors}", 
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

// Seed default locations if none exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    if (!context.Locations.Any())
    {
        var defaultLocations = new List<Location>
        {
            new Location { Name = "Oslo Senter", City = "Oslo", County = "Oslo" },
            new Location { Name = "Bergen Senter", City = "Bergen", County = "Vestland" },
            new Location { Name = "Trondheim Senter", City = "Trondheim", County = "Trøndelag" },
            new Location { Name = "Stavanger Senter", City = "Stavanger", County = "Rogaland" },
            new Location { Name = "Kristiansand Senter", City = "Kristiansand", County = "Agder" },
            new Location { Name = "Tromsø Senter", City = "Tromsø", County = "Troms og Finnmark" },
            new Location { Name = "Drammen Senter", City = "Drammen", County = "Viken" },
            new Location { Name = "Fredrikstad Senter", City = "Fredrikstad", County = "Viken" },
            new Location { Name = "Sandnes Senter", City = "Sandnes", County = "Rogaland" },
            new Location { Name = "Sarpsborg Senter", City = "Sarpsborg", County = "Viken" }
        };

        context.Locations.AddRange(defaultLocations);
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapAreaControllerRoute(
    name: "Identity",
    areaName: "Identity",
    pattern: "Identity/{controller=Home}/{action=Index}/{id?}");

app.Run();