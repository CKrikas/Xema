using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace xema.ViewModels;

public class TattooFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }

    public int CategoryId { get; set; }

    public bool IsFeatured { get; set; }
}