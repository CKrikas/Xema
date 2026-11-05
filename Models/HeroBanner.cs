namespace xema.Models;

public class HeroBanner
{
    public int Id { get; set; }

    public string Heading { get; set; } = "";

    public string? Subheading { get; set; }

    public string? ButtonText { get; set; }

    public string? ButtonLink { get; set; }

    public int MediaId { get; set; }

    public Media Media { get; set; } = null!;
}