using System.ComponentModel.DataAnnotations;

namespace LivogRøre.Models;

public class Company
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
}