namespace xema.ViewModels;

public class HeroBannerFormViewModel
{
    public int Id { get; set; }

    public string Heading { get; set; } = "";

    public string? Subheading { get; set; }

    public string? ButtonText { get; set; }

    public string? ButtonLink { get; set; }

    public string ImageUrl { get; set; } = "";
}