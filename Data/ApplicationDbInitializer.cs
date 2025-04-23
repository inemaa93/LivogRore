using LivogRøre.Models;

namespace LivogRøre.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db)
    {
        db.Database.EnsureDeleted();  // Deletes everything in the database
        db.Database.EnsureCreated();  // Recreates the database fresh
        
        // Seed locations if they don't exist
        if (!db.Locations.Any())
        {
            var locations = new List<Location>
            {
                new Location { Id = 1, Name = "Kristiansand", County = "Agder" },
                new Location { Id = 2, Name = "Arendal", County = "Agder" },
                new Location { Id = 3, Name = "Grimstad", County = "Agder" },
                new Location { Id = 4, Name = "Mandal", County = "Agder" },
                new Location { Id = 5, Name = "Farsund", County = "Agder" },
                new Location { Id = 6, Name = "Flekkefjord", County = "Agder" }
            };
            
            db.Locations.AddRange(locations);
            db.SaveChanges();
        }
        
        db.SaveChanges();
    }
}