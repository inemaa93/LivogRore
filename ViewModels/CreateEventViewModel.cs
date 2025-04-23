using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LivogRøre.ViewModels
{
    public class CreateEventViewModel
    {
        [Required]
        [Display(Name = "Tittel")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Dato")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Bilde")]
        public IFormFile? Image { get; set; }

        [Required]
        [Display(Name = "Sted")]
        public int LocationId { get; set; }

        public List<SelectListItem> Locations { get; set; } = new List<SelectListItem>();
    }
} 