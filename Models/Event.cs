using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivogRøre.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public string? CreatedBy { get; set; }

    [Required]
    public int LocationId { get; set; }

    [ForeignKey("LocationId")]
    public Location Location { get; set; } = null!;
} 