using System.ComponentModel.DataAnnotations;

namespace xema.Models;

public class Media
{
    public int Id { get; set; }

    [Required]
    public string Url { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AltText { get; set; }

    public MediaType Type { get; set; } = MediaType.Image;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}