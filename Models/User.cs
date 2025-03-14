using System.ComponentModel.DataAnnotations;

namespace LivogRøre.Models;

public class User
{
    public int Id { get; set; } = 0;
    
    [Required] // Required field
    [StringLength(10)] // Max string length
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [DataType(DataType.Date)] // Force the field to only require date, not including time
    public string? DateTime { get; set; }
}