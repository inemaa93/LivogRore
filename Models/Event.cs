using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace LivogRøre.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tittel")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Dato")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Beskrivelse")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Bilde")]
        public string? ImagePath { get; set; }

        [Display(Name = "Sted")]
        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location? Location { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        [ForeignKey("CreatedBy")]
        public IdentityUser? User { get; set; }
    }
} 