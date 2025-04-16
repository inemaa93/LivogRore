using System.ComponentModel.DataAnnotations;

namespace LivogRøre.Models;

public class Location
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string County { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
} 