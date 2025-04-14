using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LivogRøre.ViewModels;

public class CreateEventViewModel
{
    [Required(ErrorMessage = "Tittel er påkrevd.")]
    [StringLength(100, ErrorMessage = "Tittel kan maks være 100 tegn.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dato er påkrevd.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [StringLength(500, ErrorMessage = "Beskrivelse kan maks være 500 tegn.")]
    public string? Description { get; set; }

    public IFormFile? Image { get; set; }

    [Required(ErrorMessage = "Du må velge en lokasjon.")]
    public int LocationId { get; set; }

    public List<SelectListItem> Locations { get; set; } = new();
}