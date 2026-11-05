using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xema.Data;
using xema.Models;
using xema.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace xema.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var banner = await _context.HeroBanners
            .Include(h => h.Media)
            .FirstOrDefaultAsync();

        if (banner == null)
        {
            return RedirectToAction(nameof(Create));
        }

        return RedirectToAction(nameof(Edit), new { id = banner.Id });
    }

    // GET
    public IActionResult Create()
    {
        return View();
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HeroBannerFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var media = new Media
        {
            Url = model.ImageUrl,
            AltText = model.Heading
        };

        _context.Media.Add(media);
        await _context.SaveChangesAsync();

        var banner = new HeroBanner
        {
            Heading = model.Heading,
            Subheading = model.Subheading,
            ButtonText = model.ButtonText,
            ButtonLink = model.ButtonLink,
            MediaId = media.Id
        };

        _context.HeroBanners.Add(banner);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET
    public async Task<IActionResult> Edit(int id)
    {
        var banner = await _context.HeroBanners
            .Include(h => h.Media)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (banner == null)
            return NotFound();

        var vm = new HeroBannerFormViewModel
        {
            Id = banner.Id,
            Heading = banner.Heading,
            Subheading = banner.Subheading,
            ButtonText = banner.ButtonText,
            ButtonLink = banner.ButtonLink,
            ImageUrl = banner.Media.Url
        };

        return View(vm);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HeroBannerFormViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var banner = await _context.HeroBanners
            .Include(h => h.Media)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (banner == null)
            return NotFound();

        banner.Heading = model.Heading;
        banner.Subheading = model.Subheading;
        banner.ButtonText = model.ButtonText;
        banner.ButtonLink = model.ButtonLink;

        banner.Media.Url = model.ImageUrl;
        banner.Media.AltText = model.Heading;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET
    public async Task<IActionResult> Delete(int id)
    {
        var banner = await _context.HeroBanners
            .Include(h => h.Media)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (banner == null)
            return NotFound();

        return View(banner);
    }

    // POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var banner = await _context.HeroBanners
            .Include(h => h.Media)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (banner != null)
        {
            _context.HeroBanners.Remove(banner);
            _context.Media.Remove(banner.Media);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}