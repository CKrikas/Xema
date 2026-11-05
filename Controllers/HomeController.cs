using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xema.Data;
using xema.ViewModels;

namespace xema.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            HeroBanner = await _context.HeroBanners
                .Include(h => h.Media)
                .FirstOrDefaultAsync(),

            FeaturedImages = await _context.TattooImages
                .Include(t => t.Media)
                .Include(t => t.Category)
                .Where(t => t.IsFeatured)
                .ToListAsync()
        };

        return View(model);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
}