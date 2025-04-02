namespace LivogRøre.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db)
    {
        db.Database.EnsureDeleted();  // Deletes everything in the database
        db.Database.EnsureCreated();  // Recreates the database fresh
        
        db.SaveChanges();
    }
}