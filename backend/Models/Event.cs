


using System.ComponentModel.DataAnnotations;

namespace LivogRøre.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required] 
        [StringLength(100)] 
        public string Title { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public string CreatedBy { get; set; } = string.Empty;
        
        public string? ImagePath { get; set; } //Legger til seksjon for bilde
    }
}