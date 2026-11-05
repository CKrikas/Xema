using xema.Models;

namespace xema.ViewModels;

public class GalleryViewModel
{
    // Tattoos displayed on the current page
    public IEnumerable<TattooImage> Tattoos { get; set; } = [];

    // Categories for the filter buttons
    public IEnumerable<Category> Categories { get; set; } = [];

    // Current selected category
    public string? SelectedCategory { get; set; }

    // Pagination
    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; } = 9;

    public int TotalItems { get; set; }
}