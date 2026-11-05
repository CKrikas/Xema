namespace xema.ViewModels;

using xema.Models;

public class HomeViewModel
{
    public HeroBanner? HeroBanner { get; set; }

    public List<TattooImage> FeaturedImages { get; set; } = [];
}