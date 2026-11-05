using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xema.Data;
using xema.Models;
using xema.ViewModels;

namespace xema.Controllers;

public class GalleryController : Controller
{
    private readonly ApplicationDbContext _context;

    public GalleryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? category,
        int page = 1)
    {
        var vm = await BuildGalleryViewModel(category, page);

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Content(
        string? category,
        int page = 1)
    {
        var vm = await BuildGalleryViewModel(category, page);

        return PartialView("_GalleryContent", vm);
    }

    private async Task<GalleryViewModel> BuildGalleryViewModel(
        string? category,
        int page)
    {
        const int pageSize = 12;

        if (page < 1)
        {
            page = 1;
        }

        IQueryable<TattooImage> query = _context.TattooImages
            .Include(t => t.Media)
            .Include(t => t.Category);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(t =>
                t.Category != null &&
                t.Category.Name == category);
        }

        int totalItems = await query.CountAsync();

        int totalPages =
            (int)Math.Ceiling(totalItems / (double)pageSize);

        // Prevent requesting a page that does not exist.
        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var tattoos = await query
            .OrderByDescending(t => t.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new GalleryViewModel
        {
            Tattoos = tattoos,

            Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync(),

            SelectedCategory = category,

            CurrentPage = page,

            TotalPages = totalPages,

            PageSize = pageSize
        };
    }
}